using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly IAssessmentApiService assessmentApiService;
        private readonly IResultsApiService resultsApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionClient sessionClient;
        private readonly IPersistanceService persistanceService;

        public ApiService(
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionClient sessionClient,
            IPersistanceService persistanceService)
        {
            this.assessmentApiService = assessmentApiService;
            this.resultsApiService = resultsApiService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionClient = sessionClient;
            this.persistanceService = persistanceService;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                var sessionIdAndPartitionKey = GetSessionAndPartitionKey(newSessionResponse.SessionId);
                var dfcUserSession = new DfcUserSession() { Salt = "ncs", PartitionKey = sessionIdAndPartitionKey.Item1, SessionId = sessionIdAndPartitionKey.Item2 };
                sessionClient.CreateCookie(dfcUserSession, false);
            }

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var getQuestionResponse = await assessmentApiService.GetQuestion(GetSessionId(), assessmentType, questionNumber).ConfigureAwait(false);

            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer)
        {
            var questionSetResponse = await GetQuestion(assessmentType, questionNumberCounter).ConfigureAwait(false);

            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{realQuestionNumber}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(GetSessionId(), post).ConfigureAwait(false);

            return answerQuestionResponse;
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var response = await assessmentApiService.GetAssessment(GetSessionId()).ConfigureAwait(false);

            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId)
        {
            var sendEmailResponse = await assessmentApiService.SendEmail(GetSessionId(), domain, emailAddress, templateId).ConfigureAwait(false);

            return sendEmailResponse;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            return await resultsApiService.GetResults(GetSessionId()).ConfigureAwait(false);
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            return await assessmentApiService.FilterAssessment(GetSessionId(), jobCategory).ConfigureAwait(false);
        }

        public async Task<string> Reload(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);

            var assessment = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);

            persistanceService.SetValue(SessionKey.SessionId, assessment.SessionId);

            return assessment.SessionId;
        }

        private string GetSessionId()
        {
            var result=persistanceService.GetValue(SessionKey.SessionId);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException("SessionId is null or empty");
            }

            return result;
        }

        private Tuple<string, string> GetSessionAndPartitionKey(string value)
        {
            var result = new Tuple<string, string>(string.Empty, string.Empty);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var segments = value.Split("-", StringSplitOptions.RemoveEmptyEntries);
                result = new Tuple<string, string>(segments.FirstOrDefault(), segments.LastOrDefault());
            }

            return result;
        }
    }
}
