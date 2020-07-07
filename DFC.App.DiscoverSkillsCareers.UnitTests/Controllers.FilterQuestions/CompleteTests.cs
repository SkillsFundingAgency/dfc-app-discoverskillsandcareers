using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class CompleteTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;

        public CompleteTests()
        {
            mapper = A.Fake<IMapper>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            Logger = A.Fake<ILogger<FilterQuestionsController>>();

            controller = new FilterQuestionsController(Logger, mapper, FakeSessionStateService, assessmentService);
        }

        protected ILogger<FilterQuestionsController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => controller.HasSessionId()).Returns(false);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdExistsReturnsView()
        {
         
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => controller.HasSessionId()).Returns(true);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
