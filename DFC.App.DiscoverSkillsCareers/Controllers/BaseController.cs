using Dfc.Session;
using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
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
