using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter(string jobCategoryName)
        {
            return View();
        }
    }
}
