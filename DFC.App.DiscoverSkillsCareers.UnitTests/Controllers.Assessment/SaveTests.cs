using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public void WhenReturnOptionsIsEmailRedirectsToCorrectReturnType(AssessmentReturnType assessmentReturnType, string expectedRedirectUrl)
        {
            var viewModel = new AssessmentSaveRequestViewModel() { AssessmentReturnTypeId = assessmentReturnType };
            var actionResponse = AssessmentController.Save(viewModel);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/{expectedRedirectUrl}", redirectResult.Url);
        }
    }
}
