using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class JobProfileOverviewApiService : IJobProfileOverviewApiService
    {
        private readonly HttpClient httpClient;
        private readonly JobProfileOverviewServiceOptions jobProfileOverviewServiceOptions;

        public JobProfileOverviewApiService(HttpClient httpClient, JobProfileOverviewServiceOptions jobProfileOverviewServiceOptions)
        {
            this.httpClient = httpClient;
            this.jobProfileOverviewServiceOptions = jobProfileOverviewServiceOptions;
        }

        public async Task<ApiJobProfileOverview> GetOverview(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var jobProfileResponse = await httpClient.GetAsync(url!.ToString()).ConfigureAwait(false);

            var jobProfileContent = await jobProfileResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!jobProfileResponse.IsSuccessStatusCode || string.IsNullOrEmpty(jobProfileContent))
            {
                throw new InvalidOperationException($"Job Profile Overview response for {url} returned null content");
            }

            return new ApiJobProfileOverview { CanonicalName = url.Segments.LastOrDefault().Trim('/'), Html = jobProfileContent };
        }

        public async Task<List<ApiJobProfileOverview>> GetOverviews(List<string> jobProfileCanonicalNames)
        {
            List<ApiJobProfileOverview> results = new List<ApiJobProfileOverview>();

            var getTasks = jobProfileCanonicalNames.Select(x => Get(x));

            foreach (var task in getTasks)
            {
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                var taskResult = await task.ConfigureAwait(false);
                results.Add(taskResult);
            }

            return results.ToList();
        }

        private async Task<ApiJobProfileOverview> Get(string canonicalName)
        {
            var jobProfileResponse = await httpClient.GetAsync($"{jobProfileOverviewServiceOptions.BaseAddress}/{canonicalName}").ConfigureAwait(false);

            var jobProfileContent = await jobProfileResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!jobProfileResponse.IsSuccessStatusCode || string.IsNullOrEmpty(jobProfileContent))
            {
                return new ApiJobProfileOverview() { CanonicalName = canonicalName };
            }

            return new ApiJobProfileOverview { CanonicalName = canonicalName, Html = jobProfileContent };
        }
    }
}
