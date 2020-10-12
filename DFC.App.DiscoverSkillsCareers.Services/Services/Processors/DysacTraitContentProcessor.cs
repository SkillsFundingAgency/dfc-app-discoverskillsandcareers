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
    public class DysacTraitContentProcessor : BaseContentProcessor, IContentProcessor
    {
        private readonly ICmsApiService cmsApiService;
        private readonly IMapper mapper;

        public DysacTraitContentProcessor(
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
        }

        public string Type => nameof(DysacTraitContentModel);

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            var traitModel = await cmsApiService.GetItemAsync<ApiTrait>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<DysacTraitContentModel>(traitModel);

            return await ProcessContent(contentId, contentPageModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem)
        {
            return await ProcessContentItem<DysacTraitContentModel>(contentId, contentItemId, apiItem).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId)
        {
            return await RemoveContentItem<DysacTraitContentModel>(contentId, contentItemId).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            return await RemoveContent<DysacTraitContentModel>(contentId).ConfigureAwait(false);
        }
    }
}
