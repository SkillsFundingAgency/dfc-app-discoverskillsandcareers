using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.ViewComponents
{
    public class SpeakToAnAdvisorViewComponent : ViewComponent
    {
        private readonly IContentPageService<ContentItemModel> contentPageService;
        private readonly SharedContent sharedContent;

        public SpeakToAnAdvisorViewComponent(IContentPageService<ContentItemModel> contentPageService, SharedContent sharedContent)
        {
            this.contentPageService = contentPageService;
            this.sharedContent = sharedContent;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var id = new Guid(this.sharedContent.SpeakToAdvisorContentId);
            var sharedContent = await contentPageService.GetByIdAsync(id).ConfigureAwait(false);

            var model = new ContentModel();
            model.Content = sharedContent.Html_Content;

            return View(model);
        }
    }
}