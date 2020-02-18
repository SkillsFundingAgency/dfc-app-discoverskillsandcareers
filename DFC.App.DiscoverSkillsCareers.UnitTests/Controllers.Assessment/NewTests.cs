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
        public async Task IfNoSessionExistsThenRedirectsToFirstQuestion()
        {
            var questionSetName = "questionSetName1";

            A.CallTo(() => SessionService.GetValue<string>(SessionKey.SessionId)).Returns(null);

            var actionResponse = await AssessmentController.New(questionSetName).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/questionSetName1/1", redirectResult.Url);
        }
    }
}
