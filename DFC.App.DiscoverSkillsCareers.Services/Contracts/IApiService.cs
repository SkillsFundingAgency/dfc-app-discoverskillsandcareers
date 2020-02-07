using DFC.App.DiscoverSkillsCareers.Models;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IApiService
    {
        Task<bool> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string assessment, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string assessment, int questionId, string answer);

        Task<ReloadResponse> Reload();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId);
    }
}