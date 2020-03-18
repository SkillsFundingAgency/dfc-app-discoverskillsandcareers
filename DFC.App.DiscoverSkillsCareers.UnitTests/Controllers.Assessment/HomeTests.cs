using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class HomeTests
    {
        private readonly HomeController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;

        public HomeTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            controller = new HomeController(sessionService, assessmentService);
        }

        [Fact]
        public void IndexReturnsView()
        {
            var actionResponse = controller.Index();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenPostingNullModelReturnsBadRequest()
        {
            HomeIndexRequestViewModel requestViewModel = null;

            var actionResponse = await controller.Index(requestViewModel).ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenPostingInvalidModelReturnsBadRequest()
        {
            var requestViewModel = new HomeIndexRequestViewModel();
            controller.ModelState.AddModelError("key1", "some error");

            var actionResponse = await controller.Index(requestViewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenPostingValidModelRedirectsToAssessmentReturn()
        {
            var requestViewModel = new HomeIndexRequestViewModel();

            var actionResponse = await controller.Index(requestViewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }
    }
}