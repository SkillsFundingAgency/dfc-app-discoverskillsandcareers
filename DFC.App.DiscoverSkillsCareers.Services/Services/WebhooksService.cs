using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;
        private readonly IEnumerable<IContentProcessor> contentProcessors;
        private readonly IDocumentService<StaticContentItemModel> staticContentItemDocumentService;
        private readonly AutoMapper.IMapper mapper;
        private readonly Guid sharedContentId;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            ICmsApiService cmsApiService,
            IContentCacheService contentCacheService,
            IEnumerable<IContentProcessor> contentProcessors,
            AutoMapper.IMapper mapper,
            CmsApiClientOptions cmsApiClientOptions,
            IDocumentService<StaticContentItemModel> sharedContentItemDocumentService)
        {
            this.logger = logger;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
            this.contentProcessors = contentProcessors;
            this.mapper = mapper;
            this.staticContentItemDocumentService = sharedContentItemDocumentService;
            sharedContentId = Guid.Parse(cmsApiClientOptions?.ContentIds);
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            if (sharedContentId != contentId)
            {
                logger.LogInformation($"Event Id: {eventId}, is not a shared content item we are subscribed to, so no content has been processed");
                return HttpStatusCode.OK;
            }

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    return await DeleteContentAsync(contentId);

                case WebhookCacheOperation.CreateOrUpdate:
                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    return await ProcessContentAsync(url);

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Uri url)
        {
            var apiDataModel = await cmsApiService.GetItemAsync<StaticContentItemApiDataModel>(url);
            var staticContentItemModel = mapper.Map<StaticContentItemModel>(apiDataModel);

            if (staticContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(staticContentItemModel))
            {
                return HttpStatusCode.BadRequest;
            }

            var contentResult = await staticContentItemDocumentService.UpsertAsync(staticContentItemModel);

            return contentResult;
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var result = await staticContentItemDocumentService.DeleteAsync(contentId);

            return result ? HttpStatusCode.OK : HttpStatusCode.NoContent;
        }

        public bool TryValidateModel(StaticContentItemModel? sharedContentItemModel)
        {
            _ = sharedContentItemModel ?? throw new ArgumentNullException(nameof(sharedContentItemModel));

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(sharedContentItemModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(sharedContentItemModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {sharedContentItemModel.Title} - {sharedContentItemModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(
            WebhookCacheOperation webhookCacheOperation,
            Guid eventId,
            Guid contentId,
            string apiEndpoint,
            string contentType)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var contentItemCacheStatus = contentCacheService.GetContentCacheStatus(contentId);

            logger.LogInformation($"Content Item Cache Status Id:{contentId} Statuses:{JsonConvert.SerializeObject(contentItemCacheStatus)}");

            var destinationType = ContentHelpers.GetDsyacTypeFromContentType(contentType);
            var sourceType = ContentHelpers.GetApiTypeFromContentType(contentType);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    return await HandleWebhookDelete(contentId, contentItemCacheStatus, destinationType, contentType).ConfigureAwait(false);

                case WebhookCacheOperation.CreateOrUpdate:
                    return await HandleWebhookCreateOrUpdate(contentId, apiEndpoint, eventId, contentItemCacheStatus, destinationType, sourceType).ConfigureAwait(false);
                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentAsync<TModel, TDestModel>(TModel sourceType, TDestModel destType, Uri url, Guid contentId)
            where TModel : class, IBaseContentItemModel
            where TDestModel : class, IDysacContentModel
        {
            var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == destType.GetType().Name);
            return await contentProcessor.ProcessContent(url, contentId).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync<TSource, TModel>(
            TSource sourceType,
            TModel modelType,
            Uri url,
            Guid contentItemId,
            IEnumerable<ContentCacheResult> contentCacheStatuses)
             where TModel : class, IDysacContentModel
             where TSource : class, IBaseContentItemModel
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (contentCacheStatuses == null || !contentCacheStatuses.Any())
            {
                return HttpStatusCode.NoContent;
            }

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync<TSource>(sourceType.GetType(), url).ConfigureAwait(false);

            if (apiDataContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var cacheResult in contentCacheStatuses)
            {
                var contentProcessor = GetContentProcessor(cacheResult.ContentType!);

                await contentProcessor.ProcessContentItem(cacheResult.ParentContentId!.Value, contentItemId, apiDataContentItemModel).ConfigureAwait(false);
            }

            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> DeleteContentAsync<TModel>(TModel destinationType, Guid contentId, string partitionKey)
            where TModel : class, IDysacContentModel
        {
            var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == destinationType.GetType().Name);

            var result = await contentProcessor!.DeleteContentAsync(contentId, partitionKey).ConfigureAwait(false);

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync<TModel>(
            TModel destinationType,
            Guid contentItemId,
            IEnumerable<ContentCacheResult> contentCacheStatuses)
             where TModel : class, IDysacContentModel
        {
            if (contentCacheStatuses == null || !contentCacheStatuses.Any())
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var cacheResult in contentCacheStatuses)
            {
                var contentProcessor = GetContentProcessor(cacheResult.ContentType!);

                var result = await contentProcessor.DeleteContentItemAsync(
                    cacheResult.ParentContentId!.Value,
                    contentItemId,
                    cacheResult.ContentType!).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    contentCacheService.RemoveContentItem(cacheResult.ParentContentId!.Value, contentItemId);
                }
            }

            return HttpStatusCode.OK;
        }

        private IContentProcessor GetContentProcessor(string contentType)
        {
            return contentProcessors.FirstOrDefault(
                x => x.Type.ToUpperInvariant() == ContentHelpers.GetDsyacTypeFromContentType(contentType).GetType().Name.ToUpperInvariant())!;
        }

        private async Task<HttpStatusCode> HandleWebhookCreateOrUpdate(
            Guid contentId,
            string apiEndpoint,
            Guid eventId,
            IEnumerable<ContentCacheResult> contentItemCacheStatus,
            IDysacContentModel destinationType,
            IBaseContentItemModel sourceType)
        {
            if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
            {
                throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
            }

            var parentContentItems = contentItemCacheStatus.Where(x => x.Result == ContentCacheStatus.ContentItem);

            if (parentContentItems.Any())
            {
                await ProcessContentItemAsync(sourceType, destinationType, url, contentId, parentContentItems.Where(x => x.Result == ContentCacheStatus.ContentItem)).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.Any())
            {
                await ProcessContentAsync(sourceType, destinationType, url, contentId).ConfigureAwait(false);
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> HandleWebhookDelete(
            Guid contentId,
            IEnumerable<ContentCacheResult> contentItemCacheStatus,
            IDysacContentModel destinationType,
            string partitionKey)
        {
            var parentContentItems = contentItemCacheStatus.Where(x => x.Result == ContentCacheStatus.ContentItem);

            if (parentContentItems.Any())
            {
                await DeleteContentItemAsync(destinationType, contentId, parentContentItems.Where(x => x.Result == ContentCacheStatus.ContentItem)).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.Any(z => z.Result == ContentCacheStatus.Content))
            {
                await DeleteContentAsync(destinationType, contentId, partitionKey).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.All(x => x.Result == ContentCacheStatus.NotFound))
            {
                return HttpStatusCode.NotFound;
            }

            return HttpStatusCode.OK;
        }
    }
}
