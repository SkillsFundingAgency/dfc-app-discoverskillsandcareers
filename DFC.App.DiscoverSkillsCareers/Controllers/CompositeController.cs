using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class CompositeController : BaseController
    {
        public CompositeController(ISessionService sessionService)
            : base(sessionService)
        {
        }

        /// <summary>
        /// Head endpoint action method that is called by composite.
        /// </summary>
        /// <remarks>Composite needs this and send that path to this endpoint which is why we need a catch all here.</remarks>
        [Route("head/{**data}")]
        public IActionResult Head()
        {
            return View();
        }
    }
}
