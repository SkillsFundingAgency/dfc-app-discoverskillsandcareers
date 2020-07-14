using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ContentController : Controller
    {
        private readonly IContentPageService<ContentItemModel> contentPageService;

        public ContentController(IContentPageService<ContentItemModel> contentPageService)
        {
            this.contentPageService = contentPageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> GetContent()
        {
            var id = new Guid("428a6072-f7b4-4014-ad50-ccf350b68272");
            var sharedContent = contentPageService.GetByIdAsync(id).ConfigureAwait(false);

            return null;
        }
    }
}
