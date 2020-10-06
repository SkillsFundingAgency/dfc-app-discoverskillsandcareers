using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentCalculationService
    {
        Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment);
    }
}
