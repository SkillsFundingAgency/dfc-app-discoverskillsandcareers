using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Sessionstate;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly string contactUsStaxId;
        private readonly IConfiguration configuration;
        private string status;

        public HomeController(ISessionService sessionService, IAssessmentService assessmentService, IDocumentService<StaticContentItemModel> staticContentDocumentService, CmsApiClientOptions cmsApiClientOptions, ISharedContentRedisInterface sharedContentRedisInterface,
            IConfiguration configuration)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
            contactUsStaxId = cmsApiClientOptions?.ContentIds ?? throw new ArgumentNullException(nameof(cmsApiClientOptions), "ContentIds cannot be null");
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.configuration = configuration;

            status = configuration?.GetSection("contentMode:contentMode").Get<string>();

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }
        }

        public Task<IActionResult> IndexAsync()
        {
            var responseVm = new HomeIndexResponseViewModel
            {
                SpeakToAnAdviser = sharedContentRedisInterface.GetDataAsync<SharedHtml>("SharedContent/" + contactUsStaxId, status).Result.Html,
            };
            return Task.FromResult<IActionResult>(View(responseVm));
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexRequestViewModel viewModel)
        {
            if (viewModel == null || viewModel.ReferenceCode == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var responseViewModel = new HomeIndexResponseViewModel
                {
                    ReferenceCode = viewModel.ReferenceCode,
                    SpeakToAnAdviser = sharedContentRedisInterface.GetDataAsync<SharedHtml>("SharedContent/" + contactUsStaxId, status).Result.Html,
                };
                return View(responseViewModel);
            }

            var reloadResponse = await assessmentService.ReloadUsingReferenceCode(viewModel.ReferenceCode).ConfigureAwait(false);
            if (reloadResponse)
            {
                return RedirectTo("assessment/return");
            }
            else
            {
                ModelState.AddModelError("ReferenceCode", "The reference could not be found");
                var responseViewModel = new HomeIndexResponseViewModel { ReferenceCode = viewModel.ReferenceCode };
                ViewData["Title"] = "Error";
                return View(responseViewModel);
            }
        }
    }
}
