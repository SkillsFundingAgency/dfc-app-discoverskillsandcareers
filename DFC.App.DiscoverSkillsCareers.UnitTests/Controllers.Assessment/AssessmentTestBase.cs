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
        private readonly ISession session;
        private readonly IAssessmentService assessmentService;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            session = A.Fake<ISession>();
            assessmentService = A.Fake<IAssessmentService>();

            assessmentController = new AssessmentController(mapper, assessmentService, session);
        }

        protected AssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected ISession Session
        {
            get { return session; }
        }

        protected IAssessmentService ApiService
        {
            get { return assessmentService; }
        }
    }
}
