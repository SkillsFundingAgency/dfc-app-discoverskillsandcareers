using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDysacApiService
    {
        Task NewSession(AssessmentItemType assessmentType);

        Task<GetQuestionResponse> GetQuestion(AssessmentItemType assessmentType, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(AssessmentItemType assessmentType, int questionNumber, string answer);

        Task<GetAssessmentResponse> GetAssessment();

        Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId);

        Task<GetResultsResponse> GetResults();

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

        Task<string> Reload(string referenceCode);
    }
}