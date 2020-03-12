using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ILogger<AssessmentService> logger;
        private readonly NotifyOptions notifyOptions;
        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISession session;

        public AssessmentService(
            ILogger<AssessmentService> logger,
            NotifyOptions notifyOptions,
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISession session)
        {
            this.logger = logger;
            this.notifyOptions = notifyOptions;
            this.assessmentApiService = assessmentApiService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.session = session;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                session.CreateCookie(newSessionResponse.SessionId);
            }

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            var getQuestionResponse = await assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber).ConfigureAwait(false);
            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer)
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            var questionSetResponse = await GetQuestion(assessmentType, questionNumberCounter).ConfigureAwait(false);
            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{realQuestionNumber}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(sessionId, post).ConfigureAwait(false);
            return answerQuestionResponse;
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            var response = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            var sendEmailResponse = await assessmentApiService.SendEmail(sessionId, domain, emailAddress, notifyOptions.EmailTemplateId).ConfigureAwait(false);

            if (sendEmailResponse != null && !sendEmailResponse.IsSuccess)
            {
                logger.LogError($"SendEmail failed with {sendEmailResponse.Message}");
            }

            return sendEmailResponse;
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            return await assessmentApiService.FilterAssessment(sessionId, jobCategory).ConfigureAwait(false);
        }

        public async Task<string> Reload(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            var assessment = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            session.CreateCookie(assessment.SessionId);
            return assessment.SessionId;
        }

    }
}
