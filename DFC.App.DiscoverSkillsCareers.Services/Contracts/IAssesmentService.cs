using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public interface IAssesmentService<T>
        where T : IAssessmentType, new()
    {
        Task<T> CreateAssesment(DfcUserSession dfcUserSession);

        Task<GetQuestionResponse> GetQuestion(int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(int questionNumber, string answer);
    }
}