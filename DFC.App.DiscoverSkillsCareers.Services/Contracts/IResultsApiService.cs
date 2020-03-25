using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IResultsApiService
    {
        Task<GetResultsResponse> GetResults(string sessionId, string jobCategory);
    }
}
