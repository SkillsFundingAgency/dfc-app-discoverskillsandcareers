using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.SkillsHealthCheck.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        protected IActionResult RedirectToRoot()
        {
            return RedirectTo(string.Empty);
        }

        protected async Task<bool> HasSessionId()
        {
            return await sessionService.HasValidSession().ConfigureAwait(false);
        }

        protected static BreadcrumbViewModel BuildBreadcrumb()
        {
            return new BreadcrumbViewModel
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>
                {
                    new BreadcrumbItemViewModel
                    {
                        Route = "/",
                        Title = "Home",
                    },
                    new BreadcrumbItemViewModel
                    {
                        Route = "/skills-assessment",
                        Title = "Skills assessment",
                    },
                },
            };
        }
    }
}
