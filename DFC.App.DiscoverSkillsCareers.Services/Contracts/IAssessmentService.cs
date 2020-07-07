using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentService
    {
        Task<bool> NewSession(string assessmentType, Guid? sessionId);

        Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber, string sessionId);

        Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer, string sessionId);

        Task<GetAssessmentResponse> GetAssessment(string sessionId);

        Task<SendEmailResponse> SendEmail(string domain, string emailAddres, Guid? sessionIds);

        Task<SendSmsResponse> SendSms(string domain, string mobile, Guid? sessionId);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory, Guid? sessionId);

        bool ReferenceCodeExists(string referenceCode);

        Task<bool> ReloadUsingReferenceCode(string referenceCode);

        Task<bool> ReloadUsingSessionId(Guid? sessionId);
    }
}