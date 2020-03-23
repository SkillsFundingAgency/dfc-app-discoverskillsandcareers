using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            return GetOverviewsBatchedAsync(jobProfileNames);
        }

        private async Task<IEnumerable<JobProfileOverView>> GetOverviewsBatchedAsync(IEnumerable<string> jobProfileNames)
        {
            var overViews = new List<JobProfileOverView>();
            var batchSize = 25;
            int numberOfBatches = (int)Math.Ceiling((double)jobProfileNames.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var profileBatch = jobProfileNames.Skip(i * batchSize).Take(batchSize);
                var tasks = profileBatch.Select(c => GetOverviewAsync(c));
                overViews.AddRange(await Task.WhenAll(tasks).ConfigureAwait(false));
            }

            return overViews;
        }

        private async Task<JobProfileOverView> GetOverviewAsync(string cName)
        {
            var response = await httpClient.GetAsync($"segment/getbyname/{cName}").ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new JobProfileOverView() { Cname = cName, ReturnedStatusCode = HttpStatusCode.NotFound, OverViewHTML = "Not Found" };
            }
            else
            {
                response.EnsureSuccessStatusCode();
                return new JobProfileOverView() { Cname = cName, ReturnedStatusCode = response.StatusCode, OverViewHTML = await response.Content.ReadAsStringAsync().ConfigureAwait(false) };
            }
        }
    }
}
