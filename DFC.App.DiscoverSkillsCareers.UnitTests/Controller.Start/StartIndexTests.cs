using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controller.Start
{
    public class StartIndexTests : StartTestBase
    {
        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            A.CallTo(() => Session.HasValidSession()).Returns(false);

            var actionResponse = await StartController.IndexAsync().ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenModelStateIsNotValidAndSessionExistsReturnsToIndex()
        {
            var startViewModel = new StartViewModel();
            A.CallTo(() => Session.HasValidSession()).Returns(true);
            StartController.ModelState.AddModelError("Key1", "Some Error");
            StartController.TempData = A.Fake<ITempDataDictionary>();

            StartController.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => StartController.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await StartController.Index(startViewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenViewModelIsNullReturnsBadRequest()
        {
            StartViewModel viewModel = null;
            var actionResponse = await StartController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeSentToEmailIsSuccessfulRedirectsToEmailSent()
        {
            StartController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            StartController.TempData = A.Fake<ITempDataDictionary>();

            var sendEmailResponse = new SendEmailResponse() { IsSuccess = true };
            var viewModel = new StartViewModel() { Email = "someemail@gmail.com", Contact = Core.Enums.AssessmentReturnType.Email };
            A.CallTo(() => CommonService.SendEmail(A<string>.Ignored, viewModel.Email)).Returns(sendEmailResponse);

            StartController.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => StartController.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await StartController.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/start/emailsent", redirectResult.Url);
        }

        [Fact]
        public async Task WhenReferenceCodeSentToEmailModelStateIsInvalidRedirectsToView()
        {
            StartController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            StartController.TempData = A.Fake<ITempDataDictionary>();
            var sendEmailResponse = new SendEmailResponse() { IsSuccess = false };
            var viewModel = new StartViewModel() { Email = "someemail@gmail.com", Contact = Core.Enums.AssessmentReturnType.Email };
            A.CallTo(() => CommonService.SendEmail(A<string>.Ignored, viewModel.Email)).Returns(sendEmailResponse);

            StartController.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => StartController.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await StartController.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeSentToPhoneNumberModelStateIsInvalidRedirectsToIndex()
        {
            StartController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            StartController.ModelState.AddModelError("key1", "Error1");
            StartController.TempData = A.Fake<ITempDataDictionary>();

            var viewModel = new StartViewModel();
            var actionResponse = await StartController.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenReferenceCodeSenttoPhoneNumber()
        {
            StartController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            StartController.TempData = A.Fake<ITempDataDictionary>();
            var viewModel = new StartViewModel() { PhoneNumber = "07000123456", Contact = Core.Enums.AssessmentReturnType.Reference };

            await StartController.Index(viewModel).ConfigureAwait(false);

            A.CallTo(() => CommonService.SendSms(A<string>.Ignored, viewModel.PhoneNumber)).MustHaveHappenedOnceExactly();
        }
    }
}
