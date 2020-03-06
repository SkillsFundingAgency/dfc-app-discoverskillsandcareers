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

        public async Task<IEnumerable<JobProfileOverView>> GetOverviewsForProfilesAsync(IEnumerable<string> jobProfileNames)
        {
            if (jobProfileNames == null)
            {
                throw new ArgumentNullException(nameof(jobProfileNames));
            }

            var jobProfileOverViews = new List<JobProfileOverView>();

            foreach (string name in jobProfileNames)
            {
                var resposne = await httpClient.GetAsync($"segment/getbyname/{name}").ConfigureAwait(false);
                resposne.EnsureSuccessStatusCode();
                jobProfileOverViews.Add(new JobProfileOverView() { Cname = name, OverViewHTML = await resposne.Content.ReadAsStringAsync().ConfigureAwait(false)});
            }

            return jobProfileOverViews;
        }
    }
}
