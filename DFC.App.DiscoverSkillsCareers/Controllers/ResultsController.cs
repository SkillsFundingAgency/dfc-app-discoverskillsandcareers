using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : Controller
    {
        [Route("{controller}")]
        public IActionResult Show()
        {
            return View();
        }
    }
}
