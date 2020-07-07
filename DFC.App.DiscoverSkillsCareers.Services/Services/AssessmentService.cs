using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentService : IAssessmentService
    {
        private readonly NotifyOptions notifyOptions;
        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;

        public AssessmentService(
            ILogger<AssessmentService> logger,
            NotifyOptions notifyOptions,
            IAssessmentApiService assessmentApiService,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionStateService<SessionDataModel> sessionStateService)
        {
            this.Logger = logger;
            this.notifyOptions = notifyOptions;
            this.assessmentApiService = assessmentApiService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.SessionStateService = sessionStateService;
        }

        protected ILogger<AssessmentService> Logger { get; private set; }

        protected ISessionStateService<SessionDataModel> SessionStateService { get; private set; }

        public async Task<bool> NewSession(string assessmentType, Guid? sessionId)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);

            var sessionStateModel = await SessionStateService.GetAsync(sessionId.Value).ConfigureAwait(false);
            sessionStateModel.State.DysacSessionId = newSessionResponse.SessionId;
            await SessionStateService.SaveAsync(sessionStateModel).ConfigureAwait(false);

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber, string sessionId)
        {
            var getQuestionResponse = await assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber).ConfigureAwait(false);
            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer, string sessionId)
        {
            var questionSetResponse = await GetQuestion(assessmentType, questionNumberCounter, sessionId).ConfigureAwait(false);
            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{realQuestionNumber}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(sessionId, post).ConfigureAwait(false);
            return answerQuestionResponse;
        }

        public async Task<GetAssessmentResponse> GetAssessment(string sessionId)
        {
            var response = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress, Guid? sessionId)
        {
            var sendEmailResponse = await assessmentApiService.SendEmail(sessionId.ToString(), domain, emailAddress, notifyOptions.EmailTemplateId).ConfigureAwait(false);

            if (sendEmailResponse != null && !sendEmailResponse.IsSuccess)
            {
                Logger.LogError($"SendEmail failed with {sendEmailResponse.Message}");
            }

            return sendEmailResponse;
        }

        public async Task<SendSmsResponse> SendSms(string domain, string mobile, Guid? sessionId)
        {
            var sendSmsResponse = await assessmentApiService.SendSms(sessionId.ToString(), domain, mobile, notifyOptions.SmsTemplateId).ConfigureAwait(false);

            if (sendSmsResponse != null && !sendSmsResponse.IsSuccess)
            {
                Logger.LogError($"{nameof(SendSms)} failed with {sendSmsResponse.Message}");
            }

            return sendSmsResponse;
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory, Guid? sessionId)
        {
            return await assessmentApiService.FilterAssessment(sessionId.ToString(), jobCategory).ConfigureAwait(false);
        }

        public async Task<bool> ReloadUsingReferenceCode(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            return await ReloadUsingSessionId(Guid.Parse(sessionId)).ConfigureAwait(false);
        }

        public bool ReferenceCodeExists(string referenceCode)
        {
            var result = false;
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                result = true;
            }

            return result;
        }

        public async Task<bool> ReloadUsingSessionId(Guid? sessionId)
        {
            var result = false;
            var assessment = await assessmentApiService.GetAssessment(sessionId.ToString()).ConfigureAwait(false);
            if (assessment != null)
            {
                result = true;
            }

            return result;
        }
    }
}
