using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
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
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IDocumentServiceFactory documentServiceFactory;
        private readonly IContentCacheService contentCacheService;
        private readonly IEnumerable<IContentProcessor> contentProcessors;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService eventMessageService,
            ICmsApiService cmsApiService,
            IDocumentServiceFactory documentServiceFactory,
            IContentCacheService contentCacheService,
            IEnumerable<IContentProcessor> contentProcessors)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.documentServiceFactory = documentServiceFactory;
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

            //Todo - Move mappings to a helper
            var destinationType = GetDsyacTypeFromContentType(contentType);
            var sourceType = GetApiTypeFromContentType(contentType);

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

        private async Task<HttpStatusCode> HandleWebhookCreateOrUpdate(Guid contentId, string apiEndpoint, Guid eventId, IEnumerable<ContentCacheResult> contentItemCacheStatus, IDysacContentModel destinationType, IBaseContentItemModel<ApiGenericChild> sourceType)
        {
            if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
            {
                throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
            }

            var parentContentItems = contentItemCacheStatus.Where(x => x.Result == ContentCacheStatus.ContentItem);

            if (parentContentItems.Any())
            {
                await ProcessContentItemAsync(destinationType, url, contentId, parentContentItems.Where(x => x.Result == ContentCacheStatus.ContentItem)).ConfigureAwait(false);
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

        private IDysacContentModel GetDsyacTypeFromContentType(string contentType)
        {
            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return new DysacQuestionSetContentModel();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalitySkill.ToUpperInvariant())
            {
                return new DysacSkill();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return new DysacTrait();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypeJobCategory.ToUpperInvariant())
            {
                return new JobCategory();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(WebhooksService)}");
        }

        private IBaseContentItemModel<ApiGenericChild> GetApiTypeFromContentType(string contentType)
        {
            if (contentType.ToLowerInvariant() == "personalityquestionset")
            {
                return new ApiQuestionSet();
            }

            if (contentType.ToLowerInvariant() == "personalityskill")
            {
                return new ApiSkill();
            }

            if (contentType.ToLowerInvariant() == "personalitytrait")
            {
                return new ApiTrait();
            }

            if (contentType.ToLowerInvariant() == "jobcategory")
            {
                return new ApiJobCategory();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(WebhooksService)}");
        }

        public async Task<HttpStatusCode> ProcessContentAsync<TModel, TDestModel>(TModel sourceType, TDestModel destType, Uri url, Guid contentId)
            where TModel : class, IBaseContentItemModel<ApiGenericChild>
            where TDestModel : class, IDysacContentModel

        {
            var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == destType.GetType().Name);
            return await contentProcessor.ProcessContent(url, contentId).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync<TModel>(TModel modelType, Uri url, Guid contentItemId, IEnumerable<ContentCacheResult> contentCacheStatuses)
             where TModel : class, IDysacContentModel
        {
            if (contentCacheStatuses == null || !contentCacheStatuses.Any())
            {
                return HttpStatusCode.NoContent;
            }

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync<ApiGenericChild>(url).ConfigureAwait(false);

            if (apiDataContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var cacheResult in contentCacheStatuses)
            {
                var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == GetDsyacTypeFromContentType(cacheResult.ContentType).GetType().Name);

                await contentProcessor.ProcessContentItem(cacheResult.ParentContentId!.Value, contentItemId, apiDataContentItemModel).ConfigureAwait(false);
            }

            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> DeleteContentAsync<TModel>(TModel destinationType, Guid contentId)
            where TModel : class, IDysacContentModel
        {
            var result = await eventMessageService.DeleteAsync<TModel>(contentId).ConfigureAwait(false);

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
                //var contentProcessor = contentProcessors.FirstOrDefault(x => x.Type == GetDsyacTypeFromContentType(cacheResult.ContentType).GetType().Name);

                //var contentPageModel = await contentProcessor.GetByIdAsync<TModel>(cacheResult.ParentContentId!.Value).ConfigureAwait(false);

                //if (contentPageModel != null)
                //{
                //    var removedContentitem = RemoveContentItem(contentItemId, contentPageModel.GetContentItems());

                //    if (removedContentitem)
                //    {
                //        //var result = await eventMessageService.UpdateAsync<TModel>(contentPageModel).ConfigureAwait(false);

                //        //if (result == HttpStatusCode.OK)
                //        //{
                //        //    contentCacheService.RemoveContentItem(cacheResult.ParentContentId!.Value, contentItemId);
                //        //}
                //    }
                //}
            }

            return HttpStatusCode.OK;
        }
    }
}
