﻿using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class JobProfileOverviewContentProcessor : IContentProcessor
    {
        private readonly IJobProfileOverviewApiService jobProfileOverviewApiService;
        private readonly IDocumentService<DysacJobProfileOverviewContentModel> jobProfileDocumentService;

        public JobProfileOverviewContentProcessor(IJobProfileOverviewApiService jobProfileOverviewApiService, IDocumentService<DysacJobProfileOverviewContentModel> jobProfileDocumentService)
        {
            this.jobProfileOverviewApiService = jobProfileOverviewApiService;
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
            return Task.FromResult(HttpStatusCode.BadRequest);
        }

        public async Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var jobProfile = await jobProfileOverviewApiService.GetOverview(url).ConfigureAwait(false);

            var existingJobProfile = await jobProfileDocumentService.GetByIdAsync(contentId).ConfigureAwait(false);

            if (existingJobProfile != null)
            {
                existingJobProfile.Html = jobProfile.Html;
                existingJobProfile.LastCached = DateTime.UtcNow;
                var result = await jobProfileDocumentService.UpsertAsync(existingJobProfile).ConfigureAwait(false);
                return result;
            }

            var jobProfileOverview = new DysacJobProfileOverviewContentModel
            {
                Id = contentId,
                LastCached = DateTime.UtcNow,
                Html = jobProfile.Html,
            };

            var resut = await jobProfileDocumentService.UpsertAsync(jobProfileOverview).ConfigureAwait(false);
            return resut;
        }

        public Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem)
        {
            return Task.FromResult(HttpStatusCode.BadRequest);
        }
    }
}
