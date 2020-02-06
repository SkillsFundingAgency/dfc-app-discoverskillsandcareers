using DFC.App.DiscoverSkillsCareers.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        public override RedirectResult Redirect(string url)
        {
            url = $"~/{RouteName.Prefix}/" + url;
            return base.Redirect(url);
        }
    }
}
