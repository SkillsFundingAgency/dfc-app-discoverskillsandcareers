using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class LoadSessionPostTests
    {
        private readonly TestController controller;
        private readonly IAssessmentService assessmentService;

        public LoadSessionPostTests()
        {
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            Logger = A.Fake<ILogger<TestController>>();

            controller = new TestController(Logger, FakeSessionStateService, assessmentService);
        }

        protected ILogger<TestController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

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
