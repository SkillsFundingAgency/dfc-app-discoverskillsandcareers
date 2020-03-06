using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class JpOverviewApiService : IJpOverviewApiService
    {
        private readonly HttpClient httpClient;

        public JpOverviewApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<IEnumerable<JobProfileOverView>> GetOverviewsForProfilesAsync(IEnumerable<string> jobProfileNames)
        {
            if (jobProfileNames == null)
            {
                throw new ArgumentNullException(nameof(jobProfileNames));
            }

            return GetOverviewsAsync(jobProfileNames);
        }

        private async Task<IEnumerable<JobProfileOverView>> GetOverviewsAsync(IEnumerable<string> jobProfileNames)
        {
            var jobProfileOverViews = new List<JobProfileOverView>();

            foreach (string cName in jobProfileNames)
            {
                var response = await httpClient.GetAsync($"segment/getbyname/{cName}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                jobProfileOverViews.Add(new JobProfileOverView() { Cname = cName, OverViewHTML = await response.Content.ReadAsStringAsync().ConfigureAwait(false) });
            }

            return jobProfileOverViews;
        }
    }
}
