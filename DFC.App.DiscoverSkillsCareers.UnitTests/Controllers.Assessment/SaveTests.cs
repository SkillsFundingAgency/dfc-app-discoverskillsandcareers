using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class SaveTests : AssessmentTestBase
    {
        [Fact]
        public void NullViewModelReturnsBadRequest()
        {
            AssessmentSaveRequestViewModel viewModel = null;
            var actionResponse = AssessmentController.Save(viewModel);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Theory]
        [InlineData(AssessmentReturnType.Email, "assessment/email")]
        [InlineData(AssessmentReturnType.Reference, "assessment/reference")]
        public void WhenReturnOptionsIsEmailRedirectsToCorrectReturnType(AssessmentReturnType assessmentReturnType, string expectedRedirectAddress)
        {
            var viewModel = new AssessmentSaveRequestViewModel() { AssessmentReturnTypeId = assessmentReturnType };
            var actionResponse = AssessmentController.Save(viewModel);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/{expectedRedirectAddress}", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdDoesNotExistRedirectsToRoot()
        {
            var fakeSessionStateModel = A.Fake<SessionStateModel<SessionDataModel>>();
            A.CallTo(() => FakeSessionStateService.GetAsync(A<Guid>.Ignored)).Returns(fakeSessionStateModel);

            var actionResponse = await AssessmentController.Save().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }
    }
}
