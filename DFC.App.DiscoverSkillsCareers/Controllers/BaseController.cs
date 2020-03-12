using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISession session;

        public BaseController(ISession session)
        {
            this.session = session;
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

        protected async Task<bool> HasSessionId()
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(sessionId);
        }
    }
}
