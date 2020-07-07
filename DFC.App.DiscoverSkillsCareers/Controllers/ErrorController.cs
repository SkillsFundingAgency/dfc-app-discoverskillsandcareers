using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ErrorController : BaseController<ErrorController>
    {
        public ErrorController(ILogger<ErrorController> logger, ISessionStateService<SessionDataModel> sessionStateService)
            : base(logger, sessionStateService)
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
