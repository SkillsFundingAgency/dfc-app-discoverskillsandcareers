using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class CmsApiService : ICmsApiService
    {
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;
        private readonly SharedContent sharedContent;

        public CmsApiService(
            CmsApiClientOptions cmsApiClientOptions,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient, SharedContent sharedContent)
        {
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
            this.sharedContent = sharedContent;
        }

        public async Task<ContentItemModel> GetContentAsync()
        {
            var url = new Uri(
                $"{cmsApiClientOptions.BaseAddress}{sharedContent.UrlExtension}{sharedContent.SpeakToAdvisorContentId}",
                UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<ContentItemModel>(httpClient, url)
                .ConfigureAwait(false);
        }
    }
}
