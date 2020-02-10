using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IApiService
    {
        Task<bool> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string assessment, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string assessment, int questionId, string answer);

        Task<GetAssessmentResponse> GetAssessment();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId);

        Task<GetResultsResponse> GetResults();
    }
}