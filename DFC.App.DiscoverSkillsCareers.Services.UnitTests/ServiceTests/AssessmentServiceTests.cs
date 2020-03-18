using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class AssessmentServiceTests
    {
        private readonly ILogger<AssessmentService> logger;
        private readonly NotifyOptions notifyOptions;
        private readonly IAssessmentService assessmentService;
        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;

        public AssessmentServiceTests()
        {
            logger = A.Fake<ILogger<AssessmentService>>();
            notifyOptions = A.Fake<NotifyOptions>();
            assessmentApiService = A.Fake<IAssessmentApiService>();
            sessionIdToCodeConverter = A.Fake<ISessionIdToCodeConverter>();
            sessionService = A.Fake<ISessionService>();

            assessmentService = new AssessmentService(logger, notifyOptions, assessmentApiService, sessionIdToCodeConverter, sessionService);
        }

        [Fact]
        public async Task NewSessionReturnTrueWhenNewSessionIsCreated()
        {
            var assessmentType = "at1";
            var newAssessmentResponse = new NewSessionResponse() { SessionId = "s1" };
            A.CallTo(() => assessmentApiService.NewSession(assessmentType)).Returns(newAssessmentResponse);

            var response = await assessmentService.NewSession(assessmentType);

            Assert.True(response);
        }

        [Fact]
        public async Task NewSessionCallsCreateCookie()
        {
            var assessmentType = "at1";
            var newAssessmentResponse = new NewSessionResponse() { SessionId = "p1-s1" };
            A.CallTo(() => assessmentApiService.NewSession(assessmentType)).Returns(newAssessmentResponse);

            await assessmentService.NewSession(assessmentType);

            A.CallTo(() => sessionService.CreateCookie("p1-s1")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetQuestionReturnsValidResponse()
        {
            var sessionId = "session1";
            var assessmentType = "at1";
            var questionNumber = 1;
            var expectedQuestion = new GetQuestionResponse() { NextQuestionNumber = 2 };

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber)).Returns(expectedQuestion);

            var response = await assessmentService.GetQuestion(assessmentType, questionNumber);
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

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetQuestion(sessionId, assessmentType, questionResponse.QuestionNumber)).Returns(questionResponse);
            A.CallTo(() => assessmentApiService.AnswerQuestion(sessionId, answerRequest)).Returns(answerResponse);

            var response = await assessmentService.AnswerQuestion(assessmentType, questionResponse.QuestionNumber, questionResponse.QuestionNumber, answerRequest.SelectedOption);
            Assert.Equal(answerResponse.IsSuccess, response.IsSuccess);
        }

        [Fact]
        public async Task GetAssessmentCallsGetAssessmentForCurrentSession()
        {
            var sessionId = "session1";
            var assessmentResponse = new GetAssessmentResponse { SessionId = sessionId };
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).Returns(assessmentResponse);

            var response = await assessmentService.GetAssessment();

            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).MustHaveHappenedOnceExactly();
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public async Task SendEmailCallsGetSendEmailForCurrentSession()
        {
            var sessionId = "session1";
            var domain = "https://localhost";
            var emailAddress = "email@rmail.com";
            var sendEmailResponse = new SendEmailResponse() { IsSuccess = true };

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentApiService.SendEmail(sessionId, domain, emailAddress, notifyOptions.EmailTemplateId)).Returns(sendEmailResponse);

            var response = await assessmentService.SendEmail(domain, emailAddress);
            A.CallTo(() => assessmentApiService.SendEmail(sessionId, domain, emailAddress, notifyOptions.EmailTemplateId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FilterAssessmentCallsFilterAssessmentForCurrentSession()
        {
            var sessionId = "session1";
            var jobCategory = "sports";
            var filterResponse = new FilterAssessmentResponse() { SessionId = sessionId };
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentApiService.FilterAssessment(sessionId, jobCategory)).Returns(filterResponse);

            var response = await assessmentService.FilterAssessment(jobCategory);

            A.CallTo(() => assessmentApiService.FilterAssessment(sessionId, jobCategory)).MustHaveHappenedOnceExactly();
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public async Task ReloadUsingReferenceCodeCallsReloadForCurrentSession()
        {
            var referenceCode = "code1";
            var sessionIdForReferenceCode = "sessionId2";
            var asssessmentResponse = new GetAssessmentResponse() { SessionId = sessionIdForReferenceCode };
            A.CallTo(() => sessionIdToCodeConverter.GetSessionId(referenceCode)).Returns(sessionIdForReferenceCode);
            A.CallTo(() => assessmentApiService.GetAssessment(sessionIdForReferenceCode)).Returns(asssessmentResponse);

            var response = await assessmentService.ReloadUsingReferenceCode(referenceCode);

            Assert.True(response);
        }

        [Fact]
        public async Task ReloadUsingReferenceCodeCallsCreateCookie()
        {
            var referenceCode = "code1";
            var sessionIdForReferenceCode = "p1-s1";
            var asssessmentResponse = new GetAssessmentResponse() { SessionId = sessionIdForReferenceCode };
            A.CallTo(() => sessionIdToCodeConverter.GetSessionId(referenceCode)).Returns(sessionIdForReferenceCode);
            A.CallTo(() => assessmentApiService.GetAssessment(sessionIdForReferenceCode)).Returns(asssessmentResponse);

            await assessmentService.ReloadUsingReferenceCode(referenceCode);

            A.CallTo(() => sessionService.CreateCookie(sessionIdForReferenceCode)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReloadUsingSessionIdCodeCallsCreateCookie()
        {
            var sessionId = "sessionId1";
            var asssessmentResponse = new GetAssessmentResponse() { SessionId = sessionId };
            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).Returns(asssessmentResponse);

            var response = await assessmentService.ReloadUsingSessionId(sessionId);

            A.CallTo(() => sessionService.CreateCookie(sessionId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReloadUsingSessionIdThatDoesntExistReturnsFalse()
        {
            var sessionId = "sessionId1";
            GetAssessmentResponse asssessmentResponse = null;
            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).Returns(asssessmentResponse);

            var response = await assessmentService.ReloadUsingSessionId(sessionId);

            Assert.False(response);
            A.CallTo(() => sessionService.CreateCookie(sessionId)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ReloadUsingNullInvalidSessionIdReturnsFalse(string sessionId)
        {
            GetAssessmentResponse asssessmentResponse = null;
            A.CallTo(() => assessmentApiService.GetAssessment(sessionId)).Returns(asssessmentResponse);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await assessmentService.ReloadUsingSessionId(sessionId).ConfigureAwait(false));
        }
    }
}
