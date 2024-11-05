using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controller.Start
{
    public class IndexTests
    {
        private readonly StartController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;
        private readonly ICommonService commonService;
        private readonly ILogService logService;
        private readonly NotifyOptions notifyOptions;

        public IndexTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            configuration = A.Fake<IConfiguration>();
            commonService = A.Fake<ICommonService>();
            logService = A.Fake<ILogService>();
            notifyOptions = A.Fake<NotifyOptions>();
            controller = new StartController(sessionService, assessmentService, sharedContentRedisInterface, configuration, logService, commonService, notifyOptions );
        }

        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            var actionResponse = await controller.IndexAsync().ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionExistsAndModelStateIsNotValidReturnsView()
        {
            var startViewModel = new StartViewModel();
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            controller.ModelState.AddModelError("Key1", "Some Error");

            controller.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => controller.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await controller.Index(startViewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            StartViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }


        [Fact]
        public async Task WhenModelStateIsValidRedirectsToView()
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var sendEmailResponse = new SendEmailResponse() { IsSuccess = true };
            var viewModel = new StartViewModel() { Email = "someemail@gmail.com", Contact = Core.Enums.AssessmentReturnType.Email };
            A.CallTo(() => commonService.SendEmail(A<string>.Ignored, viewModel.Email)).Returns(sendEmailResponse);

            controller.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => controller.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/start/emailsent", redirectResult.Url);
        }

        [Fact]
        public async Task WhenModelStateIsInvalidRedirectsToView()
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            var sendEmailResponse = new SendEmailResponse() { IsSuccess = false };
            var viewModel = new StartViewModel() { Email = "someemail@gmail.com" , Contact = Core.Enums.AssessmentReturnType.Email };
            A.CallTo(() => commonService.SendEmail(A<string>.Ignored, viewModel.Email)).Returns(sendEmailResponse);

            controller.ObjectValidator = A.Fake<IObjectModelValidator>();
            A.CallTo(() => controller.ObjectValidator.Validate(
                A<ActionContext>.Ignored,
                A<ValidationStateDictionary>.Ignored,
                A<string>.Ignored,
                A<object>.Ignored)).DoesNothing();

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenModelStateIsInvalidRedirectsView()
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.ModelState.AddModelError("key1", "Error1");

            var viewModel = new StartViewModel();
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public async Task WhenModelStateCallsSendSms()
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            var viewModel = new StartViewModel() { PhoneNumber = "07000123456", Contact = Core.Enums.AssessmentReturnType.Reference };

            await controller.Index(viewModel).ConfigureAwait(false);

            A.CallTo(() => commonService.SendSms(A<string>.Ignored, viewModel.PhoneNumber)).MustHaveHappenedOnceExactly();
        }
    }
}
