using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {

        public ResultsController(ISessionService sessionService)
            : base(sessionService)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter()
        {
            return View();
        }
    }
}