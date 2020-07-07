using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class AssessmentTestBase
    {
        private readonly AssessmentController assessmentController;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;
        private readonly NotifyOptions notifyOptions;

        public AssessmentTestBase()
        {
            mapper = A.Fake<IMapper>();
            assessmentService = A.Fake<IAssessmentService>();
            notifyOptions = A.Fake<NotifyOptions>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            Logger = A.Fake<ILogger<AssessmentController>>();

            assessmentController = new AssessmentController(Logger, mapper, assessmentService, FakeSessionStateService, notifyOptions);
        }

        protected ILogger<AssessmentController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        protected AssessmentController AssessmentController
        {
            get { return assessmentController; }
        }

        protected IMapper Mapper
        {
            get { return mapper; }
        }

        protected ISessionStateService<SessionDataModel> Session
        {
            get { return FakeSessionStateService; }
        }

        protected IAssessmentService ApiService
        {
            get { return assessmentService; }
        }

        protected bool HasValidSession()
        {
            return assessmentController.HasSessionId();
        }
    }
}
