using Dfc.Session;
using DFC.App.DiscoverSkillsCareers.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISessionClient sessionService;

        public BaseController(ISessionClient sessionService)
        {
            this.sessionService = sessionService;
        }

        [Route("head/{**data}")]
        public IActionResult Head()
        {
            return View();
        }

        protected IActionResult RedirectTo(string relativeAddress)
        {
            relativeAddress = $"~/{RouteName.Prefix}/" + relativeAddress;
            return Redirect(relativeAddress);
        }

        protected IActionResult RedirectToRoot()
        {
            return RedirectTo(string.Empty);
        }

        protected async Task<string> GetSessionIdAsync()
        {
            return await sessionService.TryFindSessionCode().ConfigureAwait(false);
        }

        protected async Task<bool> HasSessionIdAsync()
        {
            return !string.IsNullOrWhiteSpace(await GetSessionIdAsync().ConfigureAwait(false));
        }
    }
}