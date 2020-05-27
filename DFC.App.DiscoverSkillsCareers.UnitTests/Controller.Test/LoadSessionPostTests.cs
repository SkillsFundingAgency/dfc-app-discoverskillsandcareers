using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class LoadSessionPostTests
    {
        private readonly TestController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;

        public LoadSessionPostTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            logService = A.Fake<ILogService>();

            controller = new TestController(logService, sessionService, assessmentService);
        }

        [Fact]
        public async Task NullModelReturnsBadRequest()
        {
            TestLoadSessionRequestViewModel viewModel = null;

            var actionResponse = await controller.LoadSession(viewModel).ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenModelStateIsInvalidReturnsViewResult()
        {
            var viewModel = new TestLoadSessionRequestViewModel();
            controller.ModelState.AddModelError("some error key", "some error message");

            var actionResponse = await controller.LoadSession(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenAssessmentCannotBeFoundReturnsViewResult()
        {
            var viewModel = new TestLoadSessionRequestViewModel();
            A.CallTo(() => assessmentService.ReloadUsingSessionId(viewModel.SessionId)).Returns(false);

            var actionResponse = await controller.LoadSession(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenAssessmentIsLoadedRedirectsToFirstQuestionOnAssessment()
        {
            var viewModel = new TestLoadSessionRequestViewModel();
            A.CallTo(() => assessmentService.ReloadUsingSessionId(viewModel.SessionId)).Returns(true);

            var actionResponse = await controller.LoadSession(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/1", redirectResult.Url);
        }
    }
}
