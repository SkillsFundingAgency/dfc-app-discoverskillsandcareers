using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class NewTests : AssessmentTestBase
    {
        [Fact]
        public async Task IfAssessmentTypeIsNullReturnsBadRequest()
        {
            var actionResponse = await AssessmentController.New(null).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsThenRedirectsToFirstQuestion()
        {
            var assessmentType = "short";
            var fakeSessionStateModel = A.Fake<SessionStateModel<SessionDataModel>>();
            A.CallTo(() => FakeSessionStateService.GetAsync(A<Guid>.Ignored)).Returns(fakeSessionStateModel);

            var actionResponse = await AssessmentController.New(assessmentType).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/short/1", redirectResult.Url);
        }
    }
}