using DFC.App.DiscoverSkillsCareers.Models.Result;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IResultsService
    {
        Task<GetResultsResponse> GetResults(Guid? sessionId);

        Task<GetResultsResponse> GetResultsByCategory(string jobCategory, Guid? sessionId);
    }
}
