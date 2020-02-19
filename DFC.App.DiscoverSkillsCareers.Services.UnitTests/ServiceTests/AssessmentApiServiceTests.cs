using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using FakeItEasy;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class AssessmentApiServiceTests
    {
        private readonly IAssessmentApiService assessmentApiService;
        private readonly MockHttpMessageHandler httpMessageHandler;
        private readonly HttpClient httpClient;
        private readonly ISerialiser serialiser;
        private readonly IDataProcessor<GetQuestionResponse> getQuestionResponseDataProcessor;
        private readonly IDataProcessor<GetAssessmentResponse> getAssessmentResponseDataProcessor;

        public AssessmentApiServiceTests()
        {
            serialiser = new NewtonsoftSerialiser();
            getQuestionResponseDataProcessor = A.Fake<IDataProcessor<GetQuestionResponse>>();
            getAssessmentResponseDataProcessor = A.Fake<IDataProcessor<GetAssessmentResponse>>();

            httpMessageHandler = new MockHttpMessageHandler();
            httpClient = httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://localhost");

            assessmentApiService = new AssessmentApiService(httpClient, serialiser, getQuestionResponseDataProcessor, getAssessmentResponseDataProcessor);
        }

        [Fact]
        public async Task NewSessionReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var assessmentType = "short";
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment?assessmentType={assessmentType}")
                .Respond("application/json", "{'sessionId':'session1'}");

            var newSessionResponse = await assessmentApiService.NewSession(assessmentType);
            Assert.Equal(sessionId, newSessionResponse.SessionId);
        }

        [Fact]
        public async Task GetQuestionReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var assessmentType = "short";
            var questionNumber = 2;
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment/{sessionId}/{assessmentType}/q/{questionNumber}")
                .Respond("application/json", "{'sessionId':'session1', 'currentQuestionNumber':'2', 'questionSetName':'short'}");

            var newSessionResponse = await assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber);

            Assert.Equal(sessionId, newSessionResponse.SessionId);
            Assert.Equal(assessmentType, newSessionResponse.QuestionSetName);
            Assert.Equal(questionNumber, newSessionResponse.CurrentQuestionNumber);
        }

        [Fact]
        public async Task AnswerQuestionReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var questionNumber = 2;
            var postAnswerRequest = new PostAnswerRequest() { QuestionId = questionNumber.ToString(), SelectedOption = "2" };
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment/{sessionId}")
                .Respond("application/json", "{'nextQuestionNumber':'2'}");

            var answerResponse = await assessmentApiService.AnswerQuestion(sessionId, postAnswerRequest);

            Assert.Equal(2, answerResponse.NextQuestionNumber);
        }

        [Fact]
        public async Task GetAssessmentReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment/{sessionId}/reload")
                .Respond("application/json", "{'PercentComplete':'80'}");

            var getAssessmentResponse = await assessmentApiService.GetAssessment(sessionId);

            Assert.Equal(80, getAssessmentResponse.PercentComplete);
        }

        [Fact]
        public async Task SendEmailReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var domain = "https://localhost/site1";
            var email = "sender@email.com";
            var templateId = "t1";
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment/notify/email")
                .Respond("application/json", "{'IsSuccess':'true'}");

            var sendEmailResponse = await assessmentApiService.SendEmail(sessionId, domain, email, templateId);

            Assert.True(sendEmailResponse.IsSuccess);
        }


        [Fact]
        public async Task FilterAssessmentReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var jobCategory = "sales";
            httpMessageHandler.When($"{httpClient.BaseAddress}/assessment/filtered/{sessionId}/{jobCategory}")
                .Respond("application/json", "{'sessionId':'session1'}");

            var filterAssessmentResponse = await assessmentApiService.FilterAssessment(sessionId, jobCategory);

            Assert.Equal(sessionId, filterAssessmentResponse.SessionId);
        }
    }
}
