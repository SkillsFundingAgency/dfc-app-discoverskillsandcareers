using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using Microsoft.Extensions.Logging;
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
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IDocumentServiceFactory documentServiceFactory;
        private readonly IContentCacheService contentCacheService;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService eventMessageService,
            ICmsApiService cmsApiService,
            IDocumentServiceFactory documentServiceFactory,
            IContentCacheService contentCacheService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.documentServiceFactory = documentServiceFactory;
            this.contentCacheService = contentCacheService;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            var contentItemCacheStatus = contentCacheService.CheckIsContentItem(contentId);

            //Todo - Move mappings to a helper
            var destinationType = GetDsyacTypeFromContentType(contentType);
            var sourceType = GetApiTypeFromContentType(contentType);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    switch (contentItemCacheStatus)
                    {
                        case ContentCacheStatus.ContentItem:
                            return await DeleteContentItemAsync(destinationType, contentId).ConfigureAwait(false);
                        case ContentCacheStatus.Content:
                            return await DeleteContentAsync(destinationType, contentId).ConfigureAwait(false);
                        default:
                            return HttpStatusCode.NotFound;
                    }

                case WebhookCacheOperation.CreateOrUpdate:

                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    switch (contentItemCacheStatus)
                    {
                        case ContentCacheStatus.ContentItem:
                            return await ProcessContentItemAsync(destinationType, url, contentId).ConfigureAwait(false);
                        case ContentCacheStatus.Content:
                            return await ProcessContentAsync(sourceType, destinationType, url, contentId).ConfigureAwait(false);
                        default:
                            return HttpStatusCode.NotFound;
                    }

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        private IDysacContentModel GetDsyacTypeFromContentType(string contentType)
        {
            if (contentType.ToLowerInvariant() == "personalityquestionset")
            {
                return new DysacQuestionSetContentModel();
            }

            if (contentType.ToLowerInvariant() == "personalityskill")
            {
                return new DysacSkill();
            }

            if (contentType.ToLowerInvariant() == "personalitytrait")
            {
                return new DysacTrait();
            }

            if (contentType.ToLowerInvariant() == "jobcategory")
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
            var apiDataModel = await cmsApiService.GetItemAsync<TModel, ApiGenericChild>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<TDestModel>(apiDataModel);

            if (contentPageModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(contentPageModel))
            {
                return HttpStatusCode.BadRequest;
            }

            var existingContentPageModel = await documentServiceFactory.GetDocumentService<TDestModel>().GetByIdAsync(contentId).ConfigureAwait(false);

            var contentResult = await eventMessageService.UpdateAsync<TDestModel>(contentPageModel).ConfigureAwait(false);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await eventMessageService.CreateAsync<TDestModel>(contentPageModel).ConfigureAwait(false);
            }

            if (contentResult == HttpStatusCode.OK || contentResult == HttpStatusCode.Created)
            {
                var contentItemIds = contentPageModel.AllContentItemIds;

                contentCacheService.AddOrReplace(contentId, contentItemIds);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync<TModel>(TModel modelType, Uri url, Guid contentItemId)
             where TModel : class, IDysacContentModel
        {
            var contentIds = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            if (!contentIds.Any())
            {
                return HttpStatusCode.NoContent;
            }

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync<ApiGenericChild>(url).ConfigureAwait(false);

            if (apiDataContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await documentServiceFactory.GetDocumentService<TModel>().GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    var contentItemModel = FindContentItem(contentItemId, contentPageModel.GetContentItems());

                    mapper.Map(apiDataContentItemModel, contentItemModel);
                    contentItemModel!.LastCached = DateTime.UtcNow;

                    await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);
                }
            }

            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> DeleteContentAsync<TModel>(TModel destinationType, Guid contentId)
            where TModel : class, IDysacContentModel
        {
            var result = await eventMessageService.DeleteAsync<TModel>(contentId).ConfigureAwait(false);

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync<TModel>(TModel destinationType, Guid contentItemId)
             where TModel : class, IDysacContentModel
        {
            var contentIds = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            if (!contentIds.Any())
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await documentServiceFactory.GetDocumentService<TModel>().GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    var removedContentitem = RemoveContentItem(contentItemId, contentPageModel.GetContentItems());

                    if (removedContentitem)
                    {
                        var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                        if (result == HttpStatusCode.OK)
                        {
                            contentCacheService.RemoveContentItem(contentId, contentItemId);
                        }
                    }
                }
            }

            return HttpStatusCode.OK;
        }

        public IDysacContentModel? FindContentItem(Guid contentItemId, List<IDysacContentModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == contentItemId)
                {
                    return contentItemModel;
                }

                var childContentItemModel = FindContentItem(contentItemId, contentItemModel.GetContentItems());

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }

        public bool RemoveContentItem(Guid contentItemId, List<IDysacContentModel>? items)
        {
            if (items == null || !items.Any())
            {
                return false;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == contentItemId)
                {
                    items.Remove(contentItemModel);
                    return true;
                }

                var removedContentitem = RemoveContentItem(contentItemId, contentItemModel.GetContentItems());

                if (removedContentitem)
                {
                    return removedContentitem;
                }
            }

            return false;
        }

        public bool TryValidateModel(IDysacContentModel? contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.Id} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
