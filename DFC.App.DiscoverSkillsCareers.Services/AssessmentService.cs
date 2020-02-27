using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentService : IDysacApiService
    {
        private const string MissingAssessmentType = "Missing assessment type";

        private readonly IAssessmentApiService assessmentApiService;
        private readonly IResultsApiService resultsApiService;
        private readonly IPersistanceService sessionService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;

        public AssessmentService(
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            IPersistanceService sessionService,
            ISessionIdToCodeConverter sessionIdToCodeConverter)
        {
            this.assessmentApiService = assessmentApiService;
            this.resultsApiService = resultsApiService;
            this.sessionService = sessionService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
        }

        private string SessionId => sessionService.GetValue<string>(SessionKey.SessionId);

        public async Task NewSession(Assessments assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            sessionService.SetValue(SessionKey.SessionId, newSessionResponse.SessionId);
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int questionNumber, string answer)
        {
            if (string.IsNullOrEmpty(assessmentType))
            {
                throw new ArgumentException(MissingAssessmentType, nameof(assessmentType));
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException("Missing answer", nameof(answer));
            }

            var questionSetResponse = await GetQuestion(assessmentType, questionNumber).ConfigureAwait(false);
            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{questionNumber}";
            var postAnswer = new PostAnswerRequest
            {
                QuestionId = questionIdFull,
                SelectedOption = answer,
            };

            return await assessmentApiService.AnswerQuestion(SessionId, postAnswer).ConfigureAwait(false);
        }

        public async Task<string> Reload(string referenceCode)
        {
            if (string.IsNullOrEmpty(referenceCode))
            {
                throw new ArgumentException("Missing reference code", nameof(referenceCode));
            }

            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            var assessment = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            sessionService.SetValue(SessionKey.SessionId, assessment.SessionId);

            return assessment.SessionId;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            if (string.IsNullOrEmpty(assessmentType))
            {
                throw new ArgumentException(MissingAssessmentType, nameof(assessmentType));
            }

            return await assessmentApiService.GetQuestion(SessionId, assessmentType, questionNumber).ConfigureAwait(false);
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException("Missing domain", nameof(domain));
            }

            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentException("Missing email address", nameof(emailAddress));
            }

            if (string.IsNullOrEmpty(templateId))
            {
                throw new ArgumentException("Missing template id", nameof(templateId));
            }

            return await assessmentApiService.SendEmail(SessionId, domain, emailAddress, templateId).ConfigureAwait(false);
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            if (string.IsNullOrEmpty(jobCategory))
            {
                throw new ArgumentException("Missing job categories", nameof(jobCategory));
            }

            return await assessmentApiService.FilterAssessment(SessionId, jobCategory).ConfigureAwait(false);
        }

        public async Task<GetResultsResponse> GetResults() => await resultsApiService.GetResults(SessionId).ConfigureAwait(false);

        public async Task<GetAssessmentResponse> GetAssessment() => await assessmentApiService.GetAssessment(SessionId).ConfigureAwait(false);
    }
}