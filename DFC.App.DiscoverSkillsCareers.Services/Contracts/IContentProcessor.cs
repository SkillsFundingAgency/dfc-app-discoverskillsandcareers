using DFC.App.DiscoverSkillsCareers.Models.API;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IContentProcessor
    {
        string Type { get; }

        Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId);

        Task<HttpStatusCode> ProcessContentItem(Guid parentId, Guid contentItemId, ApiGenericChild apiItem);

        Task<HttpStatusCode> RemoveContentItem(Guid contentId, Guid contentItemId);

        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);
    }
}
