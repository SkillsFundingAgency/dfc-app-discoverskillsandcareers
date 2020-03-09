using DFC.App.DiscoverSkillsCareers.Core.Constants;
using Dfc.Session;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISessionClient sessionClient;

        public BaseController(ISessionClient sessionClient)
        {
            this.sessionClient = sessionClient;
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

        protected async Task<string> GetSessionId()
        {
            var sessionId = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            return sessionId;
        }

        protected async Task<bool> HasSessionId()
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(sessionId);
        }
    }
}
