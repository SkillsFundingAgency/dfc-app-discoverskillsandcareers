using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Dfc.Session;
using Dfc.Session.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly ILogger<ApiService> logger;
        private readonly NotifyOptions notifyOptions;
        private readonly IAssessmentApiService assessmentApiService;
        private readonly IResultsApiService resultsApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionClient sessionClient;

        public ApiService(
            ILogger<ApiService> logger,
            NotifyOptions notifyOptions,
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionClient sessionClient)
        {
            this.logger = logger;
            this.notifyOptions = notifyOptions;
            this.assessmentApiService = assessmentApiService;
            this.resultsApiService = resultsApiService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionClient = sessionClient;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                CreateCookie(newSessionResponse.SessionId);
            }

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            var getQuestionResponse = await assessmentApiService.GetQuestion(sessionId, assessmentType, questionNumber).ConfigureAwait(false);
            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer)
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            var questionSetResponse = await GetQuestion(assessmentType, questionNumberCounter).ConfigureAwait(false);
            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{realQuestionNumber}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(sessionId, post).ConfigureAwait(false);
            return answerQuestionResponse;
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            var response = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            var sendEmailResponse = await assessmentApiService.SendEmail(sessionId, domain, emailAddress, notifyOptions.EmailTemplateId).ConfigureAwait(false);

            if (sendEmailResponse != null && !sendEmailResponse.IsSuccess)
            {
                logger.LogError($"SendEmail failed with {sendEmailResponse.Message}");
            }

            return sendEmailResponse;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            return await resultsApiService.GetResults(sessionId).ConfigureAwait(false);
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            var sessionId = await GetSessionId().ConfigureAwait(false);
            return await assessmentApiService.FilterAssessment(sessionId, jobCategory).ConfigureAwait(false);
        }

        public async Task<string> Reload(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            var assessment = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
            CreateCookie(assessment.SessionId);
            return assessment.SessionId;
        }

        private static Tuple<string, string> GetSessionAndPartitionKey(string value)
        {
            var result = new Tuple<string, string>(string.Empty, string.Empty);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var segments = value.Split("-", StringSplitOptions.RemoveEmptyEntries);
                result = new Tuple<string, string>(segments.ElementAtOrDefault(0), segments.ElementAtOrDefault(1));
            }

            return result;
        }

        private async Task<string> GetSessionId()
        {
            var result = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException("SessionId is null or empty");
            }

            return result;
        }

        private void CreateCookie(string sessionIdAndPartionKey)
        {
            var sessionIdAndPartitionKeyDetails = GetSessionAndPartitionKey(sessionIdAndPartionKey);
            var dfcUserSession = new DfcUserSession() { Salt = "ncs", PartitionKey = sessionIdAndPartitionKeyDetails.Item1, SessionId = sessionIdAndPartitionKeyDetails.Item2 };
            sessionClient.CreateCookie(dfcUserSession, false);
        }
    }
}
