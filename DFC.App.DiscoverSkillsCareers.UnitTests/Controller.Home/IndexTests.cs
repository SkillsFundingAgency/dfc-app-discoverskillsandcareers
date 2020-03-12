using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class IndexTests
    {
        private readonly HomeController controller;
        private readonly ISession session;
        private readonly IAssessmentService apiService;

        public IndexTests()
        {
            session = A.Fake<ISession>();
            apiService = A.Fake<IAssessmentService>();

            controller = new HomeController(session, apiService);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            HomeIndexRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeIsProvidedRedirectsToAssessment()
        {
            var viewModel = new HomeIndexRequestViewModel();

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }
    }
}
