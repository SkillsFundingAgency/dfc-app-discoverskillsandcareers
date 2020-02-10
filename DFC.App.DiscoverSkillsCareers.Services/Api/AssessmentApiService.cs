using DFC.App.DiscoverSkillsCareers.Models.Assessment;
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
        private readonly IDataProcessor<GetQuestionResponse> getQuestionResponseDataProcessor;
        private readonly IDataProcessor<GetAssessmentResponse> reloadResponseProcessor;

        public AssessmentApiService(
            HttpClient httpClient,
            ISerialiser serialiser,
            IDataProcessor<GetQuestionResponse> getQuestionResponseDataProcessor,
            IDataProcessor<GetAssessmentResponse> reloadResponseProcessor)
        {
            this.httpClient = httpClient;
            this.serialiser = serialiser;
            this.getQuestionResponseDataProcessor = getQuestionResponseDataProcessor;
            this.reloadResponseProcessor = reloadResponseProcessor;
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
            var httpResponseMessage = await httpClient.GetAsync(url).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentResponse = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = serialiser.Deserialise<GetQuestionResponse>(contentResponse);
            getQuestionResponseDataProcessor.Processor(response);
            return response;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string sessionId, PostAnswerRequest postAnswerRequest)
        {
            var url = $"{httpClient.BaseAddress}/assessment/{sessionId}";
            var httpResponseMessage = await httpClient.PostAsync(url, CreateJsonContent(postAnswerRequest)).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentResponse = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = serialiser.Deserialise<PostAnswerResponse>(contentResponse);
            return response;
        }

        public async Task<GetAssessmentResponse> GetAssessment(string sessionId)
        {
            var url = $"{httpClient.BaseAddress}/assessment/{sessionId}/reload";
            var httpResponseMessage = await httpClient.GetAsync(url).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentResponse = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = serialiser.Deserialise<GetAssessmentResponse>(contentResponse);
            reloadResponseProcessor.Processor(response);
            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string sessionId, string domain, string emailAddress, string templateId)
        {
            var url = $"{httpClient.BaseAddress}/assessment/notify/email";
            var data = new
            {
                domain,
                emailAddress,
                templateId,
                sessionId,
            };
            var httpResponseMessage = await httpClient.PostAsync(url, CreateJsonContent(data)).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentResponse = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return serialiser.Deserialise<SendEmailResponse>(contentResponse);
        }

        private StringContent CreateJsonContent(object value)
        {
            var result = new StringContent(serialiser.Serialise(value), Encoding.UTF8, MediaTypeNames.Application.Json);
            return result;
        }
    }
}