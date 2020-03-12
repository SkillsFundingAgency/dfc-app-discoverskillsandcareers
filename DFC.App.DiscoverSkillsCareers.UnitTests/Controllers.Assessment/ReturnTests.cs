using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReturnTests : AssessmentTestBase
    {
        [Fact]
        public async Task WhenReturningToAssessmentReturnsBadRequestIfAssessmentResponseIsNull()
        {
            var sessionId = "sessionId1";
            GetAssessmentResponse assessmentResponse = null;
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsFilterAssessmentAndIsCompletedThenRedirectedToResultsPage()
        {
            var sessionId = "sessionId1";
            var assessmentResponse = new GetAssessmentResponse()
            {
                IsFilterAssessment = true,
                MaxQuestionsCount = 2,
                RecordedAnswersCount = 2,
                JobCategorySafeUrl = "sports",
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/short/filterquestions/sports/complete", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsFilterAssessmentAndNotCompletedThenRedirectedToCurrentQuestion()
        {
            var sessionId = "sessionId1";
            var assessmentResponse = new GetAssessmentResponse()
            {
                IsFilterAssessment = true,
                MaxQuestionsCount = 3,
                RecordedAnswersCount = 2,
                JobCategorySafeUrl = "sports",
                CurrentQuestionNumber = 3,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/short/filterquestions/sports/3", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsNotFilterAssessmentAndIsCompletedThenRedirectedToResultsPage()
        {
            var sessionId = "sessionId1";
            var assessmentResponse = new GetAssessmentResponse()
            {
                MaxQuestionsCount = 2,
                RecordedAnswersCount = 2,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsNotFilterAssessmentAndIsNotCompletedThenRedirectedToCurrentQuestionNumber()
        {
            var sessionId = "sessionId1";
            var assessmentResponse = new GetAssessmentResponse()
            {
                MaxQuestionsCount = 4,
                RecordedAnswersCount = 2,
                QuestionSetName = "short",
                CurrentQuestionNumber = 3,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/3", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdDoesNotExistRedirectsToRoot()
        {
            string sessionId = null;
            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }
    }
}
