using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IContentProcessor
    {
        string Type { get; }

        Task<HttpStatusCode> ProcessContent(Uri url, Guid contentId);

        Task<HttpStatusCode> ProcessContentItem(Guid contentId, Guid contentItemId, IBaseContentItemModel apiItem);

        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentId, Guid contentItemId, string partitionKey);

        Task<HttpStatusCode> DeleteContentAsync(Guid contentId, string partitionKey);
    }
}
