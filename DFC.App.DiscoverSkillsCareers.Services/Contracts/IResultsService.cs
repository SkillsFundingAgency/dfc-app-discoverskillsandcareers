using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IResultsService
    {
        Task<GetResultsResponse> GetResults();

        Task<GetResultsResponse> GetResultsByCategory(string jobCategoryName);
    }
}
