using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public WebhooksService(
            ILogger<WebhooksService> logger,
            ICmsApiService cmsApiService,
            IContentCacheService contentCacheService,
            IEnumerable<IContentProcessor> contentProcessors)
        {
            this.logger = logger;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
            this.contentProcessors = contentProcessors;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var contentItemCacheStatus = contentCacheService.GetContentCacheStatus(contentId);

            var destinationType = ContentHelpers.GetDsyacTypeFromContentType(contentType);
            var sourceType = ContentHelpers.GetApiTypeFromContentType(contentType);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    return await HandleWebhookDelete(contentId, contentItemCacheStatus, destinationType).ConfigureAwait(false);

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

        public async Task<HttpStatusCode> ProcessContentItemAsync<TSource, TModel>(TSource sourceType, TModel modelType, Uri url, Guid contentItemId, IEnumerable<ContentCacheResult> contentCacheStatuses)
             where TModel : class, IDysacContentModel
             where TSource : class, IBaseContentItemModel
        {
            if (contentCacheStatuses == null || !contentCacheStatuses.Any())
            {
                return HttpStatusCode.NoContent;
            }

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync(sourceType, url).ConfigureAwait(false);

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

        public async Task<HttpStatusCode> DeleteContentAsync<TModel>(TModel destinationType, Guid contentId)
            where TModel : class, IDysacContentModel
        {
            var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == destinationType.GetType().Name);

            var result = await contentProcessor.DeleteContentAsync(contentId).ConfigureAwait(false);

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync<TModel>(TModel destinationType, Guid contentItemId, IEnumerable<ContentCacheResult> contentCacheStatuses)
             where TModel : class, IDysacContentModel
        {
            if (contentCacheStatuses == null || !contentCacheStatuses.Any())
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var cacheResult in contentCacheStatuses)
            {
                var contentProcessor = GetContentProcessor(cacheResult.ContentType!);

                var result = await contentProcessor.DeleteContentItemAsync(cacheResult.ParentContentId!.Value, contentItemId).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    contentCacheService.RemoveContentItem(cacheResult.ParentContentId!.Value, contentItemId);
                }
            }

            return HttpStatusCode.OK;
        }

        private IContentProcessor GetContentProcessor(string contentType)
        {
            return contentProcessors.FirstOrDefault(x => x.Type.ToUpperInvariant() == ContentHelpers.GetDsyacTypeFromContentType(contentType).GetType().Name.ToUpperInvariant());
        }

        private async Task<HttpStatusCode> HandleWebhookCreateOrUpdate(Guid contentId, string apiEndpoint, Guid eventId, IEnumerable<ContentCacheResult> contentItemCacheStatus, IDysacContentModel destinationType, IBaseContentItemModel sourceType)
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

            if (contentItemCacheStatus.Any(z => z.Result == ContentCacheStatus.Content))
            {
                await ProcessContentAsync(sourceType, destinationType, url, contentId).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.All(x => x.Result == ContentCacheStatus.NotFound))
            {
                return HttpStatusCode.NotFound;
            }

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> HandleWebhookDelete(Guid contentId, IEnumerable<ContentCacheResult> contentItemCacheStatus, IDysacContentModel destinationType)
        {
            var parentContentItems = contentItemCacheStatus.Where(x => x.Result == ContentCacheStatus.ContentItem);

            if (parentContentItems.Any())
            {
                await DeleteContentItemAsync(destinationType, contentId, parentContentItems.Where(x => x.Result == ContentCacheStatus.ContentItem)).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.Any(z => z.Result == ContentCacheStatus.Content))
            {
                await DeleteContentAsync(destinationType, contentId).ConfigureAwait(false);
            }

            if (contentItemCacheStatus.All(x => x.Result == ContentCacheStatus.NotFound))
            {
                return HttpStatusCode.NotFound;
            }

            return HttpStatusCode.OK;
        }
    }
}
