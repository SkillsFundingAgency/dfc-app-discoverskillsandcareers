using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IResultsService
    {
        Task<GetResultsResponse> GetResults(bool updateCollection);

        Task<GetResultsResponse> GetResultsByCategory(string jobCategoryName);

        Task<GetResultsResponse> UpdateJobCategoryCounts(DysacAssessment assessment);
    }
}
