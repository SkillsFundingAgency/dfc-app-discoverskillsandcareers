using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Dfc.Session;
using FakeItEasy;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class AssessmentTestBase
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionClient sessionClient;
        private readonly IApiService apiService;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            sessionClient = A.Fake<ISessionClient>();
            apiService = A.Fake<IApiService>();

            assessmentController = new AssessmentController(mapper, apiService, sessionClient);
        }

        protected AssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected ISessionClient SessionClient
        {
            get { return sessionClient; }
        }

        protected IApiService ApiService
        {
            get { return apiService; }
        }
    }
}
