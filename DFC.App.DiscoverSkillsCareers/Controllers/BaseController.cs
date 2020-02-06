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

        public override RedirectResult Redirect(string url)
        {
            url = $"~/{RouteName.Prefix}/" + url;
            return base.Redirect(url);
        }

        protected string GetSessionId()
        {
            return sessionService.GetValue<string>("SessionId");
        }

        protected bool HasSessionId()
        {
            return !string.IsNullOrWhiteSpace(GetSessionId());
        }
    }
}
