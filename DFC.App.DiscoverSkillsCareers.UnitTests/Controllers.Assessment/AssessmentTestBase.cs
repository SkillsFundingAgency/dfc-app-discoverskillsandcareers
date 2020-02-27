using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class AssessmentTestBase
    {
        private readonly ShortAssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IDysacApiService apiService;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            apiService = A.Fake<IDysacApiService>();

            assessmentController = new ShortAssessmentController(mapper, sessionService, apiService);
        }

        protected ShortAssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected ISessionService SessionService
        {
            get { return sessionService; }
        }

        protected IDysacApiService ApiService
        {
            get { return apiService; }
        }
    }
}
