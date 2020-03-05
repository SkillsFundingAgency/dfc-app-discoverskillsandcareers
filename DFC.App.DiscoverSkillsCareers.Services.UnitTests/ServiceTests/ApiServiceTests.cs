using Dfc.Session;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ApiServiceTests
    {
        private readonly IApiService apiService;
        private readonly IAssessmentApiService assessmentApiService;
        private readonly IResultsApiService resultsApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly IPersistanceService persistanceService;
        private readonly ISessionClient sessionClient;

        public ApiServiceTests()
        {
            assessmentApiService = A.Fake<IAssessmentApiService>();
            resultsApiService = A.Fake<IResultsApiService>();
            sessionIdToCodeConverter = A.Fake<ISessionIdToCodeConverter>();
            persistanceService = A.Fake<IPersistanceService>();
            sessionClient = A.Fake<ISessionClient>();

            apiService = new ApiService(assessmentApiService, resultsApiService, sessionIdToCodeConverter, sessionClient, persistanceService);
        }

        [Fact]
        public async Task NewSessionReturnTrueWhenNewSessionIsCreated()
        {
            var assessmentType = "at1";
            var newAssessmentResponse = new NewSessionResponse() { SessionId = "s1" };

            A.CallTo(() => assessmentApiService.NewSession(assessmentType)).Returns(newAssessmentResponse);

            var response = await apiService.NewSession(assessmentType);

            Assert.True(response);
        }


        [Fact]
        public async Task GetQuestionReturnsValidResponse()
        {
            var sessionId = "session1";
            var assessmentType = "at1";
            var questionNumber = 1;
            var expectedQuestion = new GetQuestionResponse() { NextQuestionNumber = 2 };

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber)).Returns(expectedQuestion);

            var response = await apiService.GetQuestion(assessmentType, questionNumber);

            Assert.Equal(expectedQuestion.NextQuestionNumber, response.NextQuestionNumber);
        }

        [Fact]
        public async Task AnswerQuestionReturnsValidResponse()
        {
            var sessionId = "session1";
            var assessmentType = "at1";
            var questionResponse = new GetQuestionResponse() { QuestionNumber = 1, QuestionSetVersion = $"{assessmentType}-v1" };
            var answerRequest = new PostAnswerRequest() { QuestionId = $"{assessmentType}-v1-1", SelectedOption = "2" };
            var answerResponse = A.Fake<PostAnswerResponse>();

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetQuestion(sessionId, assessmentType, questionResponse.QuestionNumber)).Returns(questionResponse);
            A.CallTo(() => assessmentApiService.AnswerQuestion(sessionId, answerRequest)).Returns(answerResponse);

            var response = await apiService.AnswerQuestion(assessmentType, questionResponse.QuestionNumber, questionResponse.QuestionNumber, answerRequest.SelectedOption);

            Assert.Equal(answerResponse.IsSuccess, response.IsSuccess);
        }

        [Fact]
        public async Task GetAssessmentCallsGetAssessmentForCurrentSession()
        {
            var sessionId = "session1";
            var assessmentResponse = new GetAssessmentResponse { SessionId = sessionId };

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).Returns(assessmentResponse);

            var response = await apiService.GetAssessment();

            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).MustHaveHappenedOnceExactly();
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public async Task SendEmailCallsGetSendEmailForCurrentSession()
        {
            var sessionId = "session1";
            var domain = "https://localhost";
            var emailAddress = "email@rmail.com";
            var templateId = "t1";
            var sendEmailResponse = new SendEmailResponse() { IsSuccess = true };

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => assessmentApiService.SendEmail(sessionId, domain, emailAddress, templateId)).Returns(sendEmailResponse);

            var response = await apiService.SendEmail(domain, emailAddress, templateId);

            A.CallTo(() => assessmentApiService.SendEmail(sessionId, domain, emailAddress, templateId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetResultsCallsGetResultsForCurrentSession()
        {
            var sessionId = "session1";
            var resultsResponse = new GetResultsResponse();

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => resultsApiService.GetResults(sessionId)).Returns(resultsResponse);

            var response = await apiService.GetResults();

            A.CallTo(() => resultsApiService.GetResults(sessionId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FilterAssessmentCallsFilterAssessmentForCurrentSession()
        {
            var sessionId = "session1";
            var jobCategory = "sports";
            var filterResponse = new FilterAssessmentResponse() { SessionId = sessionId };

            A.CallTo(() => persistanceService.GetValue(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => assessmentApiService.FilterAssessment(sessionId, jobCategory)).Returns(filterResponse);

            var response = await apiService.FilterAssessment(jobCategory);

            A.CallTo(() => assessmentApiService.FilterAssessment(sessionId, jobCategory)).MustHaveHappenedOnceExactly();
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public async Task ReloadCallsReloadForCurrentSession()
        {
            var referenceCode = "code1";
            var sessionIdForReferenceCode = "sessionId2";

            var asssessmentResponse = new GetAssessmentResponse() { SessionId = sessionIdForReferenceCode };

            A.CallTo(() => sessionIdToCodeConverter.GetSessionId(referenceCode)).Returns(sessionIdForReferenceCode);
            A.CallTo(() => assessmentApiService.GetAssessment(sessionIdForReferenceCode)).Returns(asssessmentResponse);

            var response = await apiService.Reload(referenceCode);

            Assert.Equal(sessionIdForReferenceCode, response);
        }
    }
}
