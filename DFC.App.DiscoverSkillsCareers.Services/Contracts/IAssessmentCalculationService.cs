using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentCalculationService
    {
        Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment);

        IEnumerable<JobCategoryResult> OrderJobCategoryResults(List<JobCategoryResult> resultsToOrder);
    }
}
