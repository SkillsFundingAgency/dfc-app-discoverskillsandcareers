using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class IndexGetTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;

        public IndexGetTests()
        {
            mapper = A.Fake<IMapper>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            Logger = A.Fake<ILogger<FilterQuestionsController>>();

            controller = new FilterQuestionsController(Logger, mapper, FakeSessionStateService, assessmentService);
        }

        protected ILogger<FilterQuestionsController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            FilterQuestionIndexRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            var viewModel = new FilterQuestionIndexRequestViewModel();
            A.CallTo(() => controller.HasSessionId()).Returns(false);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenNotCompletedReturnsToAssessment()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 3, RecordedAnswersCount = 2 };
            var viewModel = new FilterQuestionIndexRequestViewModel();

            A.CallTo(() => controller.HasSessionId()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAnsweredReturnsView()
        {
            var viewModel = new FilterQuestionIndexRequestViewModel() { QuestionNumber = 1 };
            A.CallTo(() => controller.HasSessionId()).Returns(true);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenNotAnsweredReturnsView()
        {
            var viewModel = new FilterQuestionIndexRequestViewModel() { JobCategoryName = "TestCategory", QuestionNumber = 0, AssessmentType = "short" };
            A.CallTo(() => controller.HasSessionId()).Returns(true);
            A.CallTo(() => assessmentService.FilterAssessment(A<string>.Ignored)).Returns(new FilterAssessmentResponse() { QuestionNumber = 1 });

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/1", redirectResult.Url);
        }
    }
}
