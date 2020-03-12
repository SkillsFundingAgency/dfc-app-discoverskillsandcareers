using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsApiService : IResultsApiService
    {
        private readonly HttpClient httpClient;
        private readonly ISerialiser serialiser;

        public ResultsApiService(
            HttpClient httpClient,
            ISerialiser serialiser)
        {
            this.httpClient = httpClient;
            this.serialiser = serialiser;
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