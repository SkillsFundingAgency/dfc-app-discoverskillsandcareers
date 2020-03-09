using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReferenceGetTests : AssessmentTestBase
    {
        [Fact]
        public async Task WhenSessionIdDoesNotExistRedirectsToRoot()
        {
            string sessionId = null;
            A.CallTo(() => SessionClient.TryFindSessionCode()).Returns(sessionId);

            var actionResponse = await AssessmentController.Reference().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task ReferenceReturnsView()
        {
            var sessionId = "session1";
            A.CallTo(() => SessionClient.TryFindSessionCode()).Returns(sessionId);

            var actionResponse = await AssessmentController.Reference().ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void ReferenceSentReturnsView()
        {
            var actionResponse = AssessmentController.ReferenceSent();

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
