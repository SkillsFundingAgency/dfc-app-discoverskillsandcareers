using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
            var uri = new Uri(relativeAddress, UriKind.Relative);
            if (!uri.IsAbsoluteUri)
            {
                relativeAddress = $"~/{RouteName.Prefix}/" + relativeAddress;
                return Redirect(relativeAddress);
            }

            return BadRequest();
        }

        protected IActionResult RedirectToRoot()
        {
            return RedirectTo(string.Empty);
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
