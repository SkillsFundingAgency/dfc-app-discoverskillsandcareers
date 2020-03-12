using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentService
    {
        Task<bool> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer);

        Task<GetAssessmentResponse> GetAssessment();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

        Task<string> Reload(string referenceCode);
    }
}