using DFC.App.DiscoverSkillsCareers.Constants;
using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController : Controller
    {
        private QuestionSetDataProvider questionSetDataProvider;

        public BaseController()
        {
            questionSetDataProvider = new QuestionSetDataProvider();
        }

        public override RedirectResult Redirect(string url)
        {
            url = $"~/{RouteName.Prefix}/" + url;
            return base.Redirect(url);
        }
    }
}
