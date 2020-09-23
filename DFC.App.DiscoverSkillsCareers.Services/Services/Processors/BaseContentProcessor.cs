﻿using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
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
        private readonly IDocumentServiceFactory documentServiceFactory;
        private readonly IMappingService mappingService;
        private readonly IEventMessageService eventMessageService;

        public BaseContentProcessor(ILogger<BaseContentProcessor> logger, IDocumentServiceFactory documentServiceFactory, IMappingService mappingService, IEventMessageService eventMessageService)
        {
            this.logger = logger;
            this.documentServiceFactory = documentServiceFactory;
            this.mappingService = mappingService;
            this.eventMessageService = eventMessageService;
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

        public async Task<HttpStatusCode> ProcessContentItem<TModel>(Guid parentId, Guid contentItemId, ApiGenericChild apiItem)
            where TModel : class, IDysacContentModel
        {
            var contentPageModel = await documentServiceFactory.GetDocumentService<TModel>().GetByIdAsync(parentId).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var contentItemModel = ContentHelpers.FindContentItem(contentItemId, contentPageModel.GetContentItems());

                mappingService.Map(contentItemModel, apiItem);
                contentItemModel!.LastCached = DateTime.UtcNow;

                await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);
            }

            return HttpStatusCode.OK;
        }
    }
}
