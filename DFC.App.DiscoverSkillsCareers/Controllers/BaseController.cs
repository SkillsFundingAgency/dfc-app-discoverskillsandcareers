using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.App.DiscoverSkillsCareers.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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


        // Double check the Routes and also the cases 
        protected static BreadcrumbViewModel BuildBreadcrumb(String route)
        {
            List<BreadcrumbItemViewModel> breadcrumbs;

            switch (route)
            {
                case "/skills-health-check/save-my-progress/breadcrumb" or "/skills-health-check/save-my-progress/" or "/skills-health-check/save-my-progress/document":
                    breadcrumbs = new List<BreadcrumbItemViewModel>
                    {
                        new BreadcrumbItemViewModel
                        {
                            Route = "/",
                            Title = "Home",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/skills-assessment/home",
                            Title = "Skills assessment",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/dysac/",
                            Title = "DYSAC",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/skills-health-check/save-my-progress?type=",
                            Title = "Save progress",
                        },
                    };
                    break;

                case "/skills-health-check/save-my-progress/getcode/breadcrumb" or "/skills-health-check/save-my-progress/getcode/document":
                    breadcrumbs = new List<BreadcrumbItemViewModel>
                    {
                        new BreadcrumbItemViewModel
                        {
                            Route = "/",
                            Title = "Home",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/skills-assessment/home",
                            Title = "Skills assessment",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/dysac/",
                            Title = "DYSAC",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/dysac/save-my-progress?type=",
                            Title = "Save progress",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/dysac/save-my-progress/getcode",
                            Title = "Reference code",
                        },
                    };
                    break;

                default:
                    breadcrumbs = new List<BreadcrumbItemViewModel>
                    {
                        new BreadcrumbItemViewModel
                        {
                            Route = "/",
                            Title = "Home",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/skills-assessment/home",
                            Title = "Skills assessment",
                        },
                        new BreadcrumbItemViewModel
                        {
                            Route = "/skills-health-check/",
                            Title = "Skills health check",
                        },
                    };
                    break;
            }

            return new BreadcrumbViewModel
            { Breadcrumbs = breadcrumbs };
        }
    }
}
