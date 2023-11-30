using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Sessionstate;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAssessmentService assessmentService;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public HomeController(ISessionService sessionService, IAssessmentService assessmentService, IDocumentService<StaticContentItemModel> staticContentDocumentService, CmsApiClientOptions cmsApiClientOptions)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
            this.staticContentDocumentService = staticContentDocumentService;
            this.cmsApiClientOptions = cmsApiClientOptions;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var responseVm = new HomeIndexResponseViewModel();
            responseVm.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
            return View(responseVm);
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
                var responseViewModel = new HomeIndexResponseViewModel { ReferenceCode = viewModel.ReferenceCode };
                responseViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
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
