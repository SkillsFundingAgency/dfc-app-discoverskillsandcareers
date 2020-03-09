using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Dfc.Session;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IApiService apiService;

        public HomeController(ISessionClient sessionClient, IApiService apiService)
            : base(sessionClient)
        {
            this.apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await apiService.Reload(viewModel.ReferenceCode).ConfigureAwait(false);

            return RedirectTo("assessment/return");
        }
    }
}
