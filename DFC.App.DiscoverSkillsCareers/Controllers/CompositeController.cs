using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class CompositeController : BaseController
    {
        private const string ViewName = "Index";

        public CompositeController(ISessionService sessionService)
            : base(sessionService)
        {
        }

        [Route("head")]
        public IActionResult Index()
        {
            var vm = CreateViewModel(PageTitle.Home);
            return View(ViewName, vm);
        }

        [Route("head/assessment/{assessmentType}/{questionNumber}")]
        [Route("head/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}")]
        public IActionResult Question(int questionNumber)
        {
            return CreateViewModelAndReturnView($"Q{questionNumber}");
        }

        [Route("head/assessment/save")]
        public IActionResult AssessmentSave()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentSave);
        }

        [Route("head/assessment/reference")]
        public IActionResult AssessmentReference()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentReference);
        }

        [Route("head/assessment/referencesent")]
        public IActionResult AssessmentReferenceSent()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentReferenceSent);
        }

        [Route("head/assessment/email")]
        public IActionResult AssessmentEmail()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentEmail);
        }

        [Route("head/assessment/emailsent")]
        public IActionResult AssessmentEmailSent()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentEmailSent);
        }

        [Route("head/loadsession")]
        public IActionResult LoadSession()
        {
            return CreateViewModelAndReturnView(PageTitle.LoadSession);
        }

        [Route("head/results")]
        [Route("head/results/{jobCategoryName}")]
        [Route("head/results/roles/{jobCategoryName}")]
        public IActionResult Results()
        {
            return CreateViewModelAndReturnView(PageTitle.Results);
        }

        [Route("head/filterquestions/complete")]
        [Route("head/assessment/complete")]
        public IActionResult AssessmentComplete()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentComplete);
        }

        [Route("head/assessment/return")]
        public IActionResult AssessmentReturn()
        {
            return CreateViewModelAndReturnView(PageTitle.AssessmentReturn);
        }

        [Route("bodytop")]
        public IActionResult BodyTopEmpty()
        {
            return View();
        }

        [Route("bodytop/{**data}")]
        public IActionResult BodyTop()
        {
            return View();
        }

        [Route("herobanner")]
        public IActionResult HeroBanner()
        {
            return View();
        }

        [Route("herobanner/{**data}")]
        public IActionResult HeroBannerEmpty()
        {
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
