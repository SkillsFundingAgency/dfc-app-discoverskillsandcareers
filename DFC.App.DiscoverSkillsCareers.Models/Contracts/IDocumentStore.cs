using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IDocumentStore
    {
        public Task<DysacAssessment?> GetAssessmentAsync(string sessionId, [CallerMemberName] string callerMemberName = "");

        public Task UpdateAssessmentAsync(DysacAssessment assessment, [CallerMemberName] string callerMemberName = "");
    }
}