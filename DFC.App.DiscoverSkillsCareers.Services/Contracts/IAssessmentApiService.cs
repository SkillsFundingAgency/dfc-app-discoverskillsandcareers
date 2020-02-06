using DFC.App.DiscoverSkillsCareers.Models;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentApiService
    {
        Task<NewSessionResponse> NewSession(string assessmentType);
    }
}
