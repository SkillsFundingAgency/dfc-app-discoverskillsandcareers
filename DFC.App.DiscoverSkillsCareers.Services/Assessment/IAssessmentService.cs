using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Assessment
{
    public interface IAssessmentService<T>
        where T : IAssessmentType, new()
    {
        Task CreateAssessmentAsync(DfcUserSession dfcUserSession = null);

        Task<GetQuestionResponse> GetQuestionAsync(int questionNumber);

        Task<PostAnswerResponse> AnswerQuestionAsync(int questionNumber, string answer);

        Task<GetAssessmentResponse> GetAssessmentAsync();
    }
}