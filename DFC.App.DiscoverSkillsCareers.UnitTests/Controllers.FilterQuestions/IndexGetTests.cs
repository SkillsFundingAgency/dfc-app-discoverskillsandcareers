using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class IndexGetTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly ISession session;
        private readonly IAssessmentService assessmentService;

        public IndexGetTests()
        {
            mapper = A.Fake<IMapper>();
            session = A.Fake<ISession>();
            assessmentService = A.Fake<IAssessmentService>();

            controller = new FilterQuestionsController(mapper, session, assessmentService);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            FilterQuestionIndexRequestViewModel viewModel = null;
            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            string sessionId = null;
            var viewModel = new FilterQuestionIndexRequestViewModel();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenNotCompletedReturnsToAssessment()
        {
            var sessionId = "sessionId1";
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 3, RecordedAnswersCount = 2 };
            var viewModel = new FilterQuestionIndexRequestViewModel();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAnsweredReturnsView()
        {
            var sessionId = "sessionId1";
            var viewModel = new FilterQuestionIndexRequestViewModel();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);

            var actionResponse = await controller.Index(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
