using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Repositories
{
    public interface IQuestionSetRepository
    {
        Task<QuestionSet> GetCurrentQuestionSet(string assessmentType);
    }
}