using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class IndexTests
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IApiService apiService;

        public IndexTests()
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
            var viewModel = new QuestionGetRequestViewModel() { QuestionId = "1", QuestionNumber = 1, AssessmentType = "name1" };

            var actionResponse = await assessmentController.Index(viewModel).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(actionResponse);
        }
    }
}
