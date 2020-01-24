using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : Controller
    {
        private QuestionSetDataProvider questionSetDataProvider;

        public AssessmentController()
        {
            questionSetDataProvider = new QuestionSetDataProvider();
        }

        public IActionResult Start(AssessmentStartRequestViewModel viewModel)
        {
            return RedirectToAction("index", "question", new { viewModel.QuestionSetName, viewModel .QuestionId});
        }

        public IActionResult Finish()
        {
            return View();
        }

        public IActionResult Continue()
        {
            return RedirectToAction("index", "question", new { QuestionSetName = "short", QuestionId = "02" });
        }

        public IActionResult Return()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Return(AssessmentReturnRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("index", "question", new { QuestionSetName = "short", QuestionId = "02" });
        }

        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(AssessmentSaveRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.SaveOption == 1)
                return RedirectToAction("Email");
            else
                return RedirectToAction("Reference");
        }

        public IActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Email(AssessmentEmailPostRequest request)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("EmailSent");
            }

            return View(request);
        }

        public IActionResult EmailSent()
        {
            return View();
        }

        public IActionResult Reference()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reference(AssessmentReferencePostRequest request)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ReferenceSent");
            }

            return View();
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }
    }
}
