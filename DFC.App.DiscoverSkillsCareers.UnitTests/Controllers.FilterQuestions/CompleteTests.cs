using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
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
        private readonly ISession session;
        private readonly IAssessmentService assessmentService;

        public CompleteTests()
        {
            mapper = A.Fake<IMapper>();
            session = A.Fake<ISession>();
            assessmentService = A.Fake<IAssessmentService>();

            controller = new FilterQuestionsController(mapper, session, assessmentService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            string sessionId = null;
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdExistsReturnsView()
        {
            var sessionId = "sessionId1";
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
