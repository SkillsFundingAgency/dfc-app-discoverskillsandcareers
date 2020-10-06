using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentService
    {
        Task<bool> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, int answer);

        Task<GetAssessmentResponse> GetAssessment();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress);

        Task<SendSmsResponse> SendSms(string domain, string mobile);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

        bool ReferenceCodeExists(string referenceCode);

        Task<bool> ReloadUsingReferenceCode(string referenceCode);

        Task<bool> ReloadUsingSessionId(string sessionId);

        Task<GetQuestionResponse> GetFilteredAssessmentQuestion(string assessment, int questionNumber);
    }
}