using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class IndexTests
    {
        private readonly HomeController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly CmsApiClientOptions cmsApiClientOptions;


        public IndexTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            staticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = Guid.NewGuid().ToString(),
            };

            controller = new HomeController(sessionService, assessmentService,staticContentDocumentService, cmsApiClientOptions);
        }

        [Fact]
        public void NullContentIdThrowsException()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new HomeController(sessionService, assessmentService, staticContentDocumentService, new CmsApiClientOptions()));

            // Assert
            Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
        }

        [Fact]
        public void NullCmsApiClientOptionsThrowsException()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new HomeController(sessionService, assessmentService, staticContentDocumentService, null));

            // Assert
            Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            HomeIndexRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task InvalidModelReturnsBadRequest()
        {
            var viewModel = new HomeIndexRequestViewModel();
            controller.ModelState.AddModelError("somekey", "somerror");

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeIsLoadedRedirectsToAssessment()
        {
            var referenceCode = "12345678";
            var viewModel = new HomeIndexRequestViewModel() { ReferenceCode = referenceCode };
            A.CallTo(() => assessmentService.ReloadUsingReferenceCode(referenceCode)).Returns(true);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task InvalidReferenceCodeReturnsViewResult()
        {
            var referenceCode = "12345678";
            var viewModel = new HomeIndexRequestViewModel() { ReferenceCode = referenceCode };
            A.CallTo(() => assessmentService.ReloadUsingReferenceCode(referenceCode)).Returns(false);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task IndexAsyncReturnsSpeaksToAdvisor()
        {
            // Arrange
            A.CallTo(() => staticContentDocumentService.GetByIdAsync(
                new Guid(cmsApiClientOptions.ContentIds),
                StaticContentItemModel.DefaultPartitionKey)).Returns(new StaticContentItemModel());

            // Act
            var result = await controller.IndexAsync() as ViewResult;
            var model = result?.Model as HomeIndexResponseViewModel;

            // Assert
            Assert.NotNull(model?.SpeakToAnAdviser);
        }
    }
}
