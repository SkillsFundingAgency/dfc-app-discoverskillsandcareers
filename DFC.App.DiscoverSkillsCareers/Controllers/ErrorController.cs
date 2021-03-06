﻿using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(ISessionService sessionService)
            : base(sessionService)
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
