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

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class IndexGetTests
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IApiService apiService;

        public IndexGetTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            apiService = A.Fake<IApiService>();

            assessmentController = new AssessmentController(mapper, sessionService, apiService);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            QuestionGetRequestViewModel viewModel = null;
            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            var viewModel = A.Fake<QuestionGetRequestViewModel>();

            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(null);

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task IfQuestionNumberIsGreaterThanMaxMaxQuestionsCountReturnsBadRequest()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 2 };

            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.GetQuestion(viewModel.AssessmentType, 1)).Returns(expectedQuestion);

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task QuestionsMustBeAnsweredInOrder()
        {
            var sessionId = "session1";
            var viewModel = new QuestionGetRequestViewModel() { QuestionNumber = 2, AssessmentType = "name1" };
            var expectedQuestion = new GetQuestionResponse() { MaxQuestionsCount = 3, QuestionNumber = 1 };

            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.GetQuestion(viewModel.AssessmentType, viewModel.QuestionNumber)).Returns(expectedQuestion);

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/name1/1", redirectResult.Url);
        }
    }
}
