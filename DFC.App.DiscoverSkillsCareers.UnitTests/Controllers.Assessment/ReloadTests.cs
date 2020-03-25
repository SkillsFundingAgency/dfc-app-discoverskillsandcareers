using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReloadTests : AssessmentTestBase
    {
        [Fact]
        public async Task WhenSessionIsNotFoundRedirectToRoot()
        {
            var sessionId = "sessionId1";
            A.CallTo(() => ApiService.ReloadUsingSessionId(sessionId)).Returns(false);

            var actionResponse = await AssessmentController.Reload(sessionId).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdIsReloadedRedirectToAssessmentReturn()
        {
            var sessionId = "sessionId1";
            A.CallTo(() => ApiService.ReloadUsingSessionId(sessionId)).Returns(true);

            var actionResponse = await AssessmentController.Reload(sessionId).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }
    }
}
