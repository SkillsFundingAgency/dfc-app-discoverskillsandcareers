using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IApiService
    {
        Task<bool> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int questionNumber, string answer);

        Task<GetAssessmentResponse> GetAssessment();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId);

        Task<GetResultsResponse> GetResults();

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

        Task<string> Reload(string referenceCode);
    }
}