﻿using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class BaseContentProcessor
    {
        private readonly ILogger<BaseContentProcessor> logger;
        private readonly IMappingService mappingService;
        private readonly IEventMessageService eventMessageService;
        private readonly IContentCacheService contentCacheService;
        private readonly IDocumentStore documentStore;

        public BaseContentProcessor(ILogger<BaseContentProcessor> logger, IDocumentStore documentStore, IMappingService mappingService, IEventMessageService eventMessageService, IContentCacheService contentCacheService)
        {
            this.logger = logger;
            this.documentStore = documentStore;
            this.mappingService = mappingService;
            this.eventMessageService = eventMessageService;
            this.contentCacheService = contentCacheService;
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
                    logger.LogError($"Error validating {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }

        public async Task<HttpStatusCode> ProcessContentItem<TModel>(Guid parentId, Guid contentItemId, IBaseContentItemModel apiItem)
            where TModel : class, IDocumentModel, IDysacContentModel
        {
            var contentPageModel = await documentStore.GetContentByIdAsync<TModel>(parentId, apiItem.ContentType!).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var contentItemModel = ContentHelpers.FindContentItem(contentItemId, contentPageModel.GetContentItems());

                mappingService.Map(contentItemModel!, apiItem);

                contentItemModel!.LastCached = DateTime.UtcNow;

                await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);
            }

            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> ProcessContent<TModel>(Guid contentId, TModel contentPageModel)
            where TModel : class, IDocumentModel, IDysacContentModel
        {
            if (contentPageModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(contentPageModel))
            {
                return HttpStatusCode.BadRequest;
            }

            var contentResult = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);
            }

            if (contentResult == HttpStatusCode.OK || contentResult == HttpStatusCode.Created)
            {
                var contentItemIds = contentPageModel.AllContentItemIds;

                contentCacheService.AddOrReplace(contentId, contentItemIds!);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> RemoveContentItem<TModel>(Guid contentId, Guid contentItemId, string partitionKey)
             where TModel : class, IDocumentModel, IDysacContentModel
        {
            var model = await documentStore.GetContentByIdAsync<TModel>(contentId, partitionKey).ConfigureAwait(false);

            if (model != null)
            {
                model.RemoveContentItem(contentItemId);

                var result = await eventMessageService.UpdateAsync<TModel>(model).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    contentCacheService.RemoveContentItem(contentId, contentItemId);
                    return HttpStatusCode.OK;
                }
            }

            return HttpStatusCode.NotFound;
        }

        public async Task<HttpStatusCode> RemoveContent<TModel>(Guid contentId, string partitionKey)
            where TModel : class, IDocumentModel
        {
            var result = await eventMessageService.DeleteAsync<TModel>(contentId, partitionKey).ConfigureAwait(false);

            return result;
        }
    }
}
