using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReferenceTests : AssessmentTestBase
    {
        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            AssessmentReferencePostRequest viewModel = null;
            var actionResponse = await AssessmentController.Reference(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenModelStateIsValidRedirectsToReferenceSent()
        {
            AssessmentController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var viewModel = new AssessmentReferencePostRequest();
            var actionResponse = await AssessmentController.Reference(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/referencesent", redirectResult.Url);
        }
    }
}
