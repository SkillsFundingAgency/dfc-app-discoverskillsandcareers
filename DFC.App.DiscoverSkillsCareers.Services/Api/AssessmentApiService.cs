using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentApiService : IAssessmentApiService
    {
        private readonly HttpClient httpClient;
        private readonly ISerialiser serialiser;

        public AssessmentApiService(HttpClient httpClient, ISerialiser serialiser)
        {
            this.httpClient = httpClient;
            this.serialiser = serialiser;
        }

        public async Task<NewSessionResponse> NewSession(string assessmentType)
        {
            var url = $"{httpClient.BaseAddress}/assessment?assessmentType={assessmentType}";
            using (var postData = new StringContent(string.Empty))
            {
                var result = await httpClient.PostAsync(url, postData).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                return serialiser.Deserialise<NewSessionResponse>(json);
            }
        }
    }
}
