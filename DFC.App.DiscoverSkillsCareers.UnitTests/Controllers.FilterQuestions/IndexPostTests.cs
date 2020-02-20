using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
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
        private readonly IApiService apiService;

        public IndexPostTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            apiService = A.Fake<IApiService>();

            controller = new FilterQuestionsController(mapper, sessionService, apiService);
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
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(null);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAnswerIsProvidedAndFilterQuestionsAreCompleteRediectsToFilterResults()
        {
            var sessionId = "session1";
            var assessmentType = "short";
            var jobCategoryName = "sales";
            var questionNumberReal = 1;
            var answer = "answer";
            var answerResponse = new PostAnswerResponse() { IsComplete = true, IsSuccess = true };
            var viewModel = new FilterQuestionPostRequestViewModel()
            {
                AssessmentType = assessmentType,
                JobCategoryName = jobCategoryName,
                Answer = answer,
                QuestionNumberReal = questionNumberReal,
            };
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.AnswerQuestion(assessmentType, questionNumberReal, answer)).Returns(answerResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/results", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAnswerIsProvidedButAnswerIsNotRegisteredReturnsView()
        {
            var sessionId = "session1";
            var assessmentType = "short";
            var jobCategoryName = "sales";
            var questionNumberReal = 1;
            var answer = "answer";
            var answerResponse = new PostAnswerResponse() { IsComplete = false, IsSuccess = false };
            var viewModel = new FilterQuestionPostRequestViewModel()
            {
                AssessmentType = assessmentType,
                JobCategoryName = jobCategoryName,
                Answer = answer,
                QuestionNumberReal = questionNumberReal,
            };
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.AnswerQuestion(assessmentType, questionNumberReal, answer)).Returns(answerResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
