using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentCalculationService
    {
        Task CalculateAssessment(DysacAssessment assessment);
    }
}
