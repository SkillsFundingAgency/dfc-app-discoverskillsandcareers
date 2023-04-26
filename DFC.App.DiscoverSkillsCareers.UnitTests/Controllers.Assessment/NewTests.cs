using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class NewTests : AssessmentTestBase
    {
        [Fact]
        public async Task IfAssessmentTypeIsNullReturnsNoContent()
        {
            var actionResponse = await AssessmentController.New(null).ConfigureAwait(false);
            Assert.IsType<NoContentResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsThenRedirectsToFirstQuestion()
        {
            var assessmentType = "short";
            string sessionId = null;

            A.CallTo(() => Session.GetSessionId()).Returns(sessionId);

            var actionResponse = await AssessmentController.New(assessmentType).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/1", redirectResult.Url);
        }
    }
}