using Dfc.Session;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(ISessionClient sessionClient)
            : base(sessionClient)
        {
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
    }
}
