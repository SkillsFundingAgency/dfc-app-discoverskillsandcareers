using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        protected IActionResult RedirectTo(string relativeAddress)
        {
            relativeAddress = $"~/{RouteName.Prefix}/" + relativeAddress;
            return Redirect(relativeAddress);
        }

        protected string GetSessionId()
        {
            return sessionService.GetValue<string>(SessionKey.SessionId);
        }

        protected bool HasSessionId()
        {
            return !string.IsNullOrWhiteSpace(GetSessionId());
        }
    }
}
