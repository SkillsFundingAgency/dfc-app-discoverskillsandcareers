using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsApiService : IResultsApiService
    {
        private readonly HttpClient httpClient;
        private readonly ISerialiser serialiser;
        private readonly IJpOverviewApiService jPOverviewAPIService;

        public ResultsApiService(
            HttpClient httpClient,
            ISerialiser serialiser,
            IJpOverviewApiService jPOverviewAPIService)
        {
            this.httpClient = httpClient;
            this.serialiser = serialiser;
            this.jPOverviewAPIService = jPOverviewAPIService;
        }

        public async Task<GetResultsResponse> GetResults(string sessionId)
        {
            var resultsResponse = await GetResults(sessionId, AssessmentTypeName.ShortAssessment).ConfigureAwait(false);
            var selectedJobprofiles = resultsResponse.JobProfiles.Select(p => p.UrlName);
            resultsResponse.JobProfilesOverviews = await jPOverviewAPIService.GetOverviewsForProfilesAsync(selectedJobprofiles).ConfigureAwait(false);
            return resultsResponse;
        }

        public async Task<GetResultsResponse> GetResults(string sessionId, string jobCategory)
        {
            var url = $"{httpClient.BaseAddress}/result/{sessionId}/{jobCategory}";
            var jsonContent = await httpClient.GetStringAsync(url).ConfigureAwait(false);
            var resultsResponse = serialiser.Deserialise<GetResultsResponse>(jsonContent);
            return resultsResponse;
        }
    }
}