using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class IndexTests
    {
        private readonly HomeController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;

        public IndexTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            configuration = A.Fake<IConfiguration>();
            controller = new HomeController(sessionService, assessmentService, sharedContentRedisInterface, configuration);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            HomeIndexRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task InvalidModelReturnsBadRequest()
        {
            var viewModel = new HomeIndexRequestViewModel();
            controller.ModelState.AddModelError("somekey", "somerror");

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeIsLoadedRedirectsToAssessment()
        {
            var referenceCode = "12345678";
            var viewModel = new HomeIndexRequestViewModel() { ReferenceCode = referenceCode };
            A.CallTo(() => assessmentService.ReloadUsingReferenceCode(referenceCode)).Returns(true);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task InvalidReferenceCodeReturnsViewResult()
        {
            var referenceCode = "12345678";
            var viewModel = new HomeIndexRequestViewModel() { ReferenceCode = referenceCode };
            A.CallTo(() => assessmentService.ReloadUsingReferenceCode(referenceCode)).Returns(false);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
