using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class CompositeController : BaseController
    {
        [Route("head")]
        public IActionResult Head()
        {
            return View();
        }
    }
}
