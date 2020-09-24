﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class DysacSkillContentProcessor : BaseContentProcessor, IContentProcessor
    {
        private readonly ICmsApiService cmsApiService;
        private readonly IMapper mapper;
        private readonly IEventMessageService eventMessageService;
        private readonly IContentCacheService contentCacheService;

        public DysacSkillContentProcessor(
            ICmsApiService cmsApiService,
            AutoMapper.IMapper mapper,
            IEventMessageService eventMessageService,
            IContentCacheService contentCacheService,
            ILogger<BaseContentProcessor> logger,
            IDocumentServiceFactory documentService,
            IMappingService mappingService)
            : base(logger, documentService, mappingService, eventMessageService, contentCacheService)
        {
            this.cmsApiService = cmsApiService;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.contentCacheService = contentCacheService;
        }

        public string Type => nameof(DysacSkillContentModel);

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            var questionModel = await cmsApiService.GetItemAsync<ApiSkill, ApiGenericChild>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<DysacSkillContentModel>(questionModel);

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

        public Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, ApiGenericChild apiItem)
        {
            throw new NotImplementedException($"{nameof(ProcessContentItem)}");
        }

        public Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId)
        {
            throw new NotImplementedException($"{nameof(DeleteContentItemAsync)}");
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            return await RemoveContent<DysacSkillContentModel>(contentId).ConfigureAwait(false);
        }
    }
}
