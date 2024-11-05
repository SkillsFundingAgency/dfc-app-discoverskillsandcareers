using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controller.Start
{
    public class StartTestBase
    {
        private readonly StartController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;
        private readonly ICommonService commonService;
        private readonly ILogService logService;
        private readonly NotifyOptions notifyOptions;

        public StartTestBase()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            configuration = A.Fake<IConfiguration>();
            commonService = A.Fake<ICommonService>();
            logService = A.Fake<ILogService>();
            notifyOptions = A.Fake<NotifyOptions>();
            controller = new StartController(sessionService, assessmentService, sharedContentRedisInterface, configuration, logService, commonService, notifyOptions);
        }

        protected StartController StartController
        {
            get { return controller; }
        }

        protected ISessionService Session
        {
            get { return sessionService; }
        }

        protected ICommonService CommonService
        {
            get { return commonService; }
        }
    }
}
