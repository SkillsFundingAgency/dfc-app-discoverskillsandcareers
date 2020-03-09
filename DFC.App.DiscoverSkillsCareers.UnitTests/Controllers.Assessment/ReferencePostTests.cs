using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReferencePostTests : AssessmentTestBase
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
            AssessmentController.TempData = A.Fake<ITempDataDictionary>();

            var viewModel = new AssessmentReferencePostRequest();
            var actionResponse = await AssessmentController.Reference(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/referencesent", redirectResult.Url);
        }

        [Fact]
        public async Task WhenModelStateIsInvalidRedirectsView()
        {
            AssessmentController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            AssessmentController.ModelState.AddModelError("key1", "Error1");

            var viewModel = new AssessmentReferencePostRequest();
            var actionResponse = await AssessmentController.Reference(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
