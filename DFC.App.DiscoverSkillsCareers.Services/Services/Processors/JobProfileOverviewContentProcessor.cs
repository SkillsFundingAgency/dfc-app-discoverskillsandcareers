using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class JobProfileOverviewContentProcessor : IContentProcessor
    {
        private readonly IJobProfileOverviewApiService jobProfileOverviewApiService;
        private readonly IDocumentStore documentStore;

        public JobProfileOverviewContentProcessor(
            IJobProfileOverviewApiService jobProfileOverviewApiService,
            IDocumentStore documentStore)
        {
            this.jobProfileOverviewApiService = jobProfileOverviewApiService;
            this.documentStore = documentStore;
        }

        public string Type => nameof(DysacJobProfileOverviewContentModel);

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, string partitionKey)
        {
            var result = await documentStore.DeleteContentAsync<DysacJobProfileOverviewContentModel>(contentId, partitionKey).ConfigureAwait(false);
            return result ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        public Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId, string partitionKey)
        {
            return Task.FromResult(HttpStatusCode.BadRequest);
        }

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var jobProfile = await jobProfileOverviewApiService.GetOverview(url).ConfigureAwait(false);

            var existingJobProfile = await documentStore.GetContentByIdAsync<DysacJobProfileOverviewContentModel>(contentId, "JobProfileOverview").ConfigureAwait(false);

            if (existingJobProfile != null)
            {
                existingJobProfile.Html = jobProfile.Html;
                existingJobProfile.LastCached = DateTime.UtcNow;
                var result = await documentStore.UpdateContentAsync(existingJobProfile).ConfigureAwait(false);
                return result;
            }

            var jobProfileOverview = new DysacJobProfileOverviewContentModel
            {
                Id = contentId,
                LastCached = DateTime.UtcNow,
                Html = jobProfile.Html,
            };

            var resut = await documentStore.UpdateContentAsync(jobProfileOverview).ConfigureAwait(false);
            return resut;
        }

        public Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem)
        {
            return Task.FromResult(HttpStatusCode.BadRequest);
        }
    }
}
