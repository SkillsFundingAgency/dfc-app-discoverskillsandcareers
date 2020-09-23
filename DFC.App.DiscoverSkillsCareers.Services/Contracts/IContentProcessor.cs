using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IContentProcessor
    {
        string Type { get; }

        Task<HttpStatusCode> Process(Uri url, Guid contentId);
    }
}
