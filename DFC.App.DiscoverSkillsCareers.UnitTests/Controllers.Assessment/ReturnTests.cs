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
            GetAssessmentResponse assessmentResponse = null;
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsFilterAssessmentAndIsCompletedThenRedirectedToResultsPage()
        {
            var assessmentResponse = new GetAssessmentResponse
            {
                IsFilterAssessment = true,
                MaxQuestionsCount = 2,
                RecordedAnswersCount = 2,
                JobCategorySafeUrl = "sports",
                AtLeastOneAnsweredFilterQuestion = true,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results/roles/sports", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsFilterAssessmentAndNotCompletedThenRedirectedToCurrentQuestion()
        {
            var assessmentResponse = new GetAssessmentResponse()
            {
                IsFilterAssessment = true,
                MaxQuestionsCount = 3,
                RecordedAnswersCount = 2,
                CurrentFilterAssessmentCode = "sports",
                CurrentQuestionNumber = 3,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/short/filterquestions/sports/3", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsNotFilterAssessmentAndIsCompletedThenRedirectedToResultsPage()
        {
            var assessmentResponse = new GetAssessmentResponse()
            {
                MaxQuestionsCount = 2,
                RecordedAnswersCount = 2,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReturningToAssessmentAndAssessmentIsNotFilterAssessmentAndIsNotCompletedThenRedirectedToCurrentQuestionNumber()
        {
            var assessmentResponse = new GetAssessmentResponse()
            {
                MaxQuestionsCount = 4,
                RecordedAnswersCount = 2,
                CurrentQuestionNumber = 3,
            };
            A.CallTo(() => ApiService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/3", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdDoesNotExistRedirectsToRoot()
        {
            A.CallTo(() => Session.HasValidSession()).Returns(false);

            var actionResponse = await AssessmentController.Return().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }
    }
}
