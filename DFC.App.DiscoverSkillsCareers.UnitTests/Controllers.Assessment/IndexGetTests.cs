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
            var viewModel = A.Fake<QuestionGetRequestViewModel>();

            A.CallTo(() => Session.HasValidSession()).Returns(false);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task IfQuestionDoesNotExistsReturnsBadRequest()
        {
            var viewModel = new QuestionGetRequestViewModel() { AssessmentType = "at1", QuestionNumber = 1 };
            GetQuestionResponse expectedQuestion = null;

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfQuestionNumberIsGreaterThanMaxMaxQuestionsCountReturnsBadRequest()
        {
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 2 };

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, 1)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfAssessmentIsCompleteRedirectsToResults()
        {
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 2, IsComplete = true };

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results", redirectResult.Url);
        }

        [Fact]
        public async Task GetAssessmentWhenAssessmentDoesntExistReturnsBadResponse()
        {
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse();
            GetAssessmentResponse expectedAssessment = null;

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);
            A.CallTo(() => ApiService.GetAssessment()).Returns(expectedAssessment);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task QuestionsMustBeAnsweredInOrder()
        {
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 3, QuestionNumber = 1 };
            var expectedAssessment = new GetAssessmentResponse() { CurrentQuestionNumber = 1 };

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);
            A.CallTo(() => ApiService.GetAssessment()).Returns(expectedAssessment);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/name1/1", redirectResult.Url);
        }

        [Fact]
        public async Task PercentageCompletePropertyGetsUpdatedWhenGoesBackToPreviousQuestion()
        {
            var expectedPercentageComplete = 33;
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 3, QuestionNumber = 1 };
            var expectedAssessment = new GetAssessmentResponse() { CurrentQuestionNumber = 3, MaxQuestionsCount = 3 };

            A.CallTo(() => Session.HasValidSession()).Returns(true);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);
            A.CallTo(() => ApiService.GetAssessment()).Returns(expectedAssessment);
            A.CallTo(() => Mapper.Map<QuestionGetResponseViewModel>(expectedQuestion)).Returns(new QuestionGetResponseViewModel { PercentageComplete = expectedPercentageComplete });

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<ViewResult>(actionResponse);

            var viewResult = actionResponse as ViewResult;
            var model = viewResult.Model as QuestionGetResponseViewModel;

            Assert.Equal(expectedPercentageComplete, model.PercentageComplete);
        }
    }
}
