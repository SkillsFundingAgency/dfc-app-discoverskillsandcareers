using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class IndexPostTests
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IApiService apiService;

        public IndexPostTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            apiService = A.Fake<IApiService>();

            assessmentController = new AssessmentController(mapper, sessionService, apiService);
        }

        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            QuestionPostRequestViewModel viewModel = null;
            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public async Task IfNoSessionExistsRedirectedToRoot()
        {
            var viewModel = A.Fake<QuestionPostRequestViewModel>();

            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(null);

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<RedirectResult>(actionResponse);

            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task IfNoAnswerIsProvidedReturnsView()
        {
            var sessionId = "session1";
            var viewModel = new QuestionPostRequestViewModel() { QuestionNumber = 3, AssessmentType = "name1" };

            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
