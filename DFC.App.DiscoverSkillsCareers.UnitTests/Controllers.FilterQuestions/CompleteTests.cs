using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class CompleteTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;

        public CompleteTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            logService = A.Fake<ILogService>();

            controller = new FilterQuestionsController(logService, mapper, sessionService, assessmentService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdExistsReturnsView()
        {
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
