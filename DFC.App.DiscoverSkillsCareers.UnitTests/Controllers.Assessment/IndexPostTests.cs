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
    public class IndexPostTests : AssessmentTestBase
    {
        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            QuestionPostRequestViewModel viewModel = null;
            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            string sessionId = null;
            var viewModel = A.Fake<QuestionPostRequestViewModel>();

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task IfQuestionDoesNotExistReturnsBadRequest()
        {
            var sessionId = "session1";
            var viewModel = new QuestionPostRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1" };
            GetQuestionResponse question = null;

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(question);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoAnswerIsProvidedReturnsView()
        {
            var sessionId = "session1";
            var viewModel = new QuestionPostRequestViewModel() { QuestionNumber = 3, AssessmentType = "short" };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task IfAnswerIsValidMovesToNextQuestion()
        {
            var sessionId = "session1";
            var answerRequest = new QuestionPostRequestViewModel() { QuestionNumber = 3, AssessmentType = "short", Answer = "answer1" };
            var currentQuestion = new GetQuestionResponse() { MaxQuestionsCount = 10, CurrentQuestionNumber = 3, NextQuestionNumber = 4 };
            var answerResponse = new PostAnswerResponse() { IsSuccess = true, NextQuestionNumber = 4 };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(answerRequest.AssessmentType, answerRequest.QuestionNumber)).Returns(currentQuestion);
            A.CallTo(() => ApiService.AnswerQuestion(answerRequest.AssessmentType, answerRequest.QuestionNumber, answerRequest.QuestionNumber, answerRequest.Answer)).Returns(answerResponse);

            var actionResponse = await AssessmentController.Index(answerRequest).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/4", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAllAnswersAreProvidedAssessmentIsCompleted()
        {
            var sessionId = "session1";
            var answerRequest = new QuestionPostRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1", Answer = "answer1" };
            var currentQuestion = new GetQuestionResponse();
            var answerResponse = new PostAnswerResponse() { IsSuccess = true, IsComplete = true };

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => ApiService.GetQuestion(answerRequest.AssessmentType, answerRequest.QuestionNumber)).Returns(currentQuestion);
            A.CallTo(() => ApiService.AnswerQuestion(answerRequest.AssessmentType, answerRequest.QuestionNumber, answerRequest.QuestionNumber, answerRequest.Answer)).Returns(answerResponse);

            var actionResponse = await AssessmentController.Index(answerRequest).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/complete", redirectResult.Url);
        }
    }
}
