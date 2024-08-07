﻿using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class CompositeController : BaseController
    {
        private const string ViewName = "Index";
        private readonly ILogService logService;

        public CompositeController(ISessionService sessionService,  ILogService logService)
            : base(sessionService)
        {
            this.logService = logService;
        }

        [Route("head")]
        [Route("head/assessment/new")]
        public IActionResult Index()
        {
            logService.LogInformation($"head/assessment/new Index is called");
            var vm = CreateViewModel(PageTitle.Home);
            return View(ViewName, vm);
        }

        [Route("head/assessment/{assessmentType}/{questionNumber}")]
        [Route("head/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}")]
        public IActionResult Question(int questionNumber)
        {
            logService.LogInformation($"{questionNumber} Question is called");
            return CreateViewModelAndReturnView($"Q{questionNumber}");
        }

        [Route("head/{assessmentType}/filterquestions/{jobCategoryName}/complete")]
        [Route("head/assessment/complete")]
        public IActionResult AssessmentComplete()
        {
            logService.LogInformation($"AssessmentComplete ");
            return CreateViewModelAndReturnView(PageTitle.AssessmentComplete);
        }

        [Route("head/assessment/save")]
        public IActionResult AssessmentSave()
        {
            logService.LogInformation($"AssessmentComplete called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentSave);
        }

        [Route("head/assessment/reference")]
        public IActionResult AssessmentReference()
        {
            logService.LogInformation($"AssessmentReference {PageTitle.AssessmentReference} called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentReference);
        }

        [Route("head/assessment/referencesent")]
        public IActionResult AssessmentReferenceSent()
        {
            logService.LogInformation($"AssessmentReferenceSent {PageTitle.AssessmentReferenceSent} called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentReferenceSent);
        }

        [Route("head/assessment/email")]
        public IActionResult AssessmentEmail()
        {
            logService.LogInformation($"AssessmentEmail {PageTitle.AssessmentEmail} called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentEmail);
        }

        [Route("head/assessment/emailsent")]
        public IActionResult AssessmentEmailSent()
        {
            logService.LogInformation($"AssessmentEmailSent {PageTitle.AssessmentEmailSent} called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentEmailSent);
        }

        [Route("head/loadsession")]
        public IActionResult LoadSession()
        {
            logService.LogInformation($"LoadSession {PageTitle.LoadSession} called");
            return CreateViewModelAndReturnView(PageTitle.LoadSession);
        }

        [Route("head/results")]
        [Route("head/results/{jobCategoryName}")]
        [Route("head/results/roles/{jobCategoryName}")]
        public IActionResult Results()
        {
            logService.LogInformation($"Results {PageTitle.Results} called");
            return CreateViewModelAndReturnView(PageTitle.Results);
        }

        [Route("head/assessment/return")]
        public IActionResult AssessmentReturn()
        {
            logService.LogInformation($"AssessmentReturn {PageTitle.AssessmentReturn} called");
            return CreateViewModelAndReturnView(PageTitle.AssessmentReturn);
        }

        [Route("bodytop")]
        public IActionResult BodyTopEmpty()
        {
            logService.LogInformation($"BodyTopEmpty called");
            return View();
        }

        [Route("bodytop/{**data}")]
        public IActionResult BodyTop()
        {
            logService.LogInformation($"BodyTop called");
            return View();
        }

        [Route("bodytop/assessment/{assessmentType}/1")]
        [Route("bodytop/assessment/complete")]
        public IActionResult BodyTopBackToStart()
        {
            logService.LogInformation($"BodyTopBackToStart called");
            return View();
        }

        [Route("bodytop/assessment/{assessmentType}/{questionNumber}")]
        public IActionResult BodyTopQuestions(string assessmentType, int questionNumber)
        {
            logService.LogInformation($"BodyTopBackToStart assessmentType{assessmentType} questionNumber{questionNumber} called");
            var previousQuestionNumber = questionNumber - 1;
            if (previousQuestionNumber < 0)
            {
                previousQuestionNumber = 0;
            }

            var vm = new BodyTopQuestionsViewModel()
            {
                AssessmentType = assessmentType,
                PreviousQuestionNumber = previousQuestionNumber,
            };
            return View(vm);
        }

        [Route("herobanner")]
        public IActionResult HeroBanner()
        {
            logService.LogInformation($"HeroBanner called");
            return View();
        }

        [Route("bodyfooter/{**data}")]
        public IActionResult BodyFooter()
        {
            logService.LogInformation($"BodyFooter called");
            return View();
        }

        [Route("herobanner/{**data}")]
        public IActionResult HeroBannerEmpty()
        {
            logService.LogInformation($"HeroBannerEmpty called");
            return Content(string.Empty);
        }

        private static HeadResponseViewModel CreateViewModel(string title)
        {
            var result = new HeadResponseViewModel
            {
                Title = $"{title} | {PageTitle.Dysac} | {PageTitle.Ncs}",
            };

            return result;
        }

        private IActionResult CreateViewModelAndReturnView(string title)
        {
            var vm = CreateViewModel(title);
            return View(ViewName, vm);
        }
    }
}
