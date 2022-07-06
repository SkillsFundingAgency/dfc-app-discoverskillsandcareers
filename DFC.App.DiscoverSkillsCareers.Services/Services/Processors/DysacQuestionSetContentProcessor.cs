using AutoMapper;
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
    public class DysacQuestionSetContentProcessor : BaseContentProcessor, IContentProcessor
    {
        private readonly ICmsApiService cmsApiService;
        private readonly IMapper mapper;

        public DysacQuestionSetContentProcessor(
            ICmsApiService cmsApiService,
            IMapper mapper,
            IEventMessageService eventMessageService,
            IContentCacheService contentCacheService,
            ILogger<BaseContentProcessor> logger,
            IDocumentStore documentStore,
            IMappingService mappingService)
            : base(logger, documentStore, mappingService, eventMessageService, contentCacheService)
        {
            this.cmsApiService = cmsApiService;
            this.mapper = mapper;
        }

        public string Type => nameof(DysacQuestionSetContentModel);

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            var questionModel = await cmsApiService.GetItemAsync<ApiQuestionSet>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<DysacQuestionSetContentModel>(questionModel);

            return await ProcessContent(contentId, contentPageModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem)
        {
            return await ProcessContentItem<DysacQuestionSetContentModel>(contentId, contentItemId, apiItem).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId, string partitionKey)
        {
            return await RemoveContentItem<DysacQuestionSetContentModel>(contentId, contentItemId, partitionKey).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, string partitionKey)
        {
            return await RemoveContent<DysacQuestionSetContentModel>(contentId, partitionKey).ConfigureAwait(false);
        }
    }
}
