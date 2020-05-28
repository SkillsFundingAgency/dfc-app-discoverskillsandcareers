using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class AssessmentTestBase
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            logService = A.Fake<ILogService>();

            assessmentController = new AssessmentController(logService, mapper, assessmentService, sessionService);
        }

        protected AssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected ISessionService Session
        {
            get { return sessionService; }
        }

        protected IAssessmentService ApiService
        {
            get { return assessmentService; }
        }
    }
}
