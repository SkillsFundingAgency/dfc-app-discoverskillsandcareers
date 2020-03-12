using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class IndexGetTests : AssessmentTestBase
    {
        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            QuestionGetRequestViewModel viewModel = null;
            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            string sessionId = null;
            var viewModel = A.Fake<QuestionGetRequestViewModel>();

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task IfQuestionDoesNotExistsReturnsBadRequest()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { AssessmentType = "at1", QuestionNumber = 1 };
            GetQuestionResponse expectedQuestion = null;

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfQuestionNumberIsGreaterThanMaxMaxQuestionsCountReturnsBadRequest()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 2 };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, 1)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfAssessmentIsCompleteRedirectsToResults()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 2, IsComplete = true };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results", redirectResult.Url);
        }

        [Fact]
        public async Task QuestionsMustBeAnsweredInOrder()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 3, QuestionNumber = 1 };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/name1/1", redirectResult.Url);
        }
    }
}
