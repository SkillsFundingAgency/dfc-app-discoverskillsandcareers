using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class IndexPostTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;

        public IndexPostTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            logService = A.Fake<ILogService>();

            controller = new FilterQuestionsController(logService, mapper, sessionService, assessmentService);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            FilterQuestionPostRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            var viewModel = new FilterQuestionPostRequestViewModel();
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Theory]
        [InlineData(true, "complete")]
        [InlineData(false, "2")]
        public async Task WhenAnsweredRedirectsToCorrectNextView(bool isComplete, string expectedRedirect)
        {
            var assessmentType = "short";
            var jobCategoryName = "sales";
            var questionNumberReal = 1;
            var questionNumberCounter = 1;
            var answer = 3;
            var answerResponse = new PostAnswerResponse() { IsComplete = isComplete, IsSuccess = true };
            var viewModel = new FilterQuestionPostRequestViewModel()
            {
                AssessmentType = assessmentType,
                JobCategoryName = jobCategoryName,
                Answer = answer,
                QuestionNumberReal = questionNumberReal,
                QuestionNumberCounter = questionNumberCounter,
            };

            A.CallTo(() => sessionService.HasValidSession()).Returns(true);

            A.CallTo(() => assessmentService.AnswerQuestion(jobCategoryName, questionNumberReal, questionNumberReal, answer)).Returns(answerResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/{assessmentType}/filterquestions/{jobCategoryName}/{expectedRedirect}", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAnswerIsProvidedButAnswerIsNotRegisteredReturnsView()
        {
            var assessmentType = "short";
            var jobCategoryName = "sales";
            var questionNumberReal = 1;
            var answer = 3;
            var answerResponse = new PostAnswerResponse() { IsComplete = false, IsSuccess = false };
            var viewModel = new FilterQuestionPostRequestViewModel()
            {
                AssessmentType = assessmentType,
                JobCategoryName = jobCategoryName,
                Answer = answer,
                QuestionNumberReal = questionNumberReal,
            };

            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.AnswerQuestion(assessmentType, questionNumberReal, questionNumberReal, answer)).Returns(answerResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task IfModelStateInvalidReturnsBackToQuestion()
        {
            var viewModel = new FilterQuestionPostRequestViewModel();
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            controller.ModelState.AddModelError("key", "Test Error");

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
