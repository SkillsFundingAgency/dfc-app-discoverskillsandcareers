using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class AssessmentTestBase
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly IPersistanceService persistanceService;
        private readonly IApiService apiService;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            persistanceService = A.Fake<IPersistanceService>();
            apiService = A.Fake<IApiService>();

            assessmentController = new AssessmentController(mapper, persistanceService, apiService);
        }

        protected AssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected IPersistanceService PersistanceService
        {
            get { return persistanceService; }
        }

        protected IApiService ApiService
        {
            get { return apiService; }
        }
    }
}
