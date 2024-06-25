using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentCalculationService
    {
        Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment);
        //149589 commented out
        //IEnumerable<JobCategoryResult> OrderJobCategoryResults(List<JobCategoryResult> resultsToOrder);
    }
}
