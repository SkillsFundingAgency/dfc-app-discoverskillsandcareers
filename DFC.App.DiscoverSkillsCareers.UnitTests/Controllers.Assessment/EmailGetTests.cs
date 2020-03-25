using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class EmailGetTests : AssessmentTestBase
    {
        [Fact]
        public async Task WhenSessionDoesNotExistRedirectsToRoot()
        {
            A.CallTo(() => Session.HasValidSession()).Returns(false);

            var actionResponse = await AssessmentController.Email().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionExistsReturnsView()
        {
            A.CallTo(() => Session.HasValidSession()).Returns(true);

            var actionResponse = await AssessmentController.Email().ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
