using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class JobProfileOverviewContentProcessor : IContentProcessor
    {
        private readonly HttpClient httpClient;
        private readonly IDocumentService<DysacJobProfileOverviewContentModel> jobProfileDocumentService;

        public JobProfileOverviewContentProcessor(HttpClient httpClient, IDocumentService<DysacJobProfileOverviewContentModel> jobProfileDocumentService)
        {
            this.httpClient = httpClient;
            this.jobProfileDocumentService = jobProfileDocumentService;
        }

        public string Type => nameof(DysacJobProfileOverviewContentModel);

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var result = await jobProfileDocumentService.DeleteAsync(contentId).ConfigureAwait(false);
            return result ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        public Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            

            var existingJobProfile = await jobProfileDocumentService.GetByIdAsync(contentId).ConfigureAwait(false);

            if (existingJobProfile != null)
            {
                existingJobProfile.Html = jobProfileContent;
                existingJobProfile.LastCached = DateTime.UtcNow;
                var result = await jobProfileDocumentService.UpsertAsync(existingJobProfile).ConfigureAwait(false);
                return result;
            }

            var jobProfileOverview = new DysacJobProfileOverviewContentModel
            {
                Id = contentId,
                LastCached = DateTime.UtcNow,
                Html = jobProfileContent,
            };

            var resut = await jobProfileDocumentService.UpsertAsync(jobProfileOverview).ConfigureAwait(false);
            return resut;
        }

        public Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem)
        {
            throw new NotImplementedException();
        }
    }
}
