using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Assessment
{
    public interface IAssessmentService<T>
        where T : IAssessmentType, new()
    {
        Task<DfcUserSession> CreateAssessment();

        Task<GetQuestionResponse> GetQuestion(int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(int questionNumber, string answer);
    }
}