using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class EmailTests : AssessmentTestBase
    {
        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            AssessmentEmailPostRequest viewModel = null;
            var actionResponse = await AssessmentController.Email(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenModelStateIsValidRedirectsToSendEmail()
        {
            AssessmentController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var viewModel = new AssessmentEmailPostRequest();
            var actionResponse = await AssessmentController.Email(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/emailsent", redirectResult.Url);
        }
    }
}
