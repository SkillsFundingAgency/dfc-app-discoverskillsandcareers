using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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
                var contentResponse = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                return serialiser.Deserialise<NewSessionResponse>(contentResponse);
            }
        }

        public async Task<GetQuestionResponse> GetQuestion(string sessionId, string assessment, int questionNumber)
        {
            var url = $"{httpClient.BaseAddress}/assessment/{sessionId}/{assessment}/q/{questionNumber}";
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
            var contentResponse = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return serialiser.Deserialise<GetQuestionResponse>(contentResponse);
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string sessionId, PostAnswerRequest postAnswerRequest)
        {
            var url = $"{httpClient.BaseAddress}/assessment/{sessionId}";
            var result = await httpClient.PostAsync(url, CreateJsonContent(postAnswerRequest)).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
            var contentResponse = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return serialiser.Deserialise<PostAnswerResponse>(contentResponse);
        }

        private StringContent CreateJsonContent(object value)
        {
            var result = new StringContent(serialiser.Serialise(value), Encoding.UTF8, MediaTypeNames.Application.Json);
            return result;
        }
    }
}
