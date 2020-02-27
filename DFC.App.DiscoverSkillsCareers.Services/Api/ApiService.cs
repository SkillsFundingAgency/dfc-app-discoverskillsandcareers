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
        private readonly IPersistanceService persistanceService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionClient sessionClient;

        public ApiService(
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            IPersistanceService sessionService,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionClient sessionClient)
        {
            this.assessmentApiService = assessmentApiService;
            this.resultsApiService = resultsApiService;
            this.persistanceService = sessionService;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionClient = sessionClient;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                var dfcUserSession = CreateDfcUserSession(newSessionResponse.SessionId);

                sessionClient.CreateCookie(dfcUserSession, false);
            }

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            Validate();

            var getQuestionResponse = await assessmentApiService.GetQuestion(GetSessionId(), assessmentType, questionNumber).ConfigureAwait(false);

            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int questionNumber, string answer)
        {
            Validate();

            var questionSetResponse = await GetQuestion(assessmentType, questionNumber).ConfigureAwait(false);

            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{questionNumber}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(GetSessionId(), post).ConfigureAwait(false);

            return answerQuestionResponse;
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            Validate();

            var response = await assessmentApiService.GetAssessment(GetSessionId()).ConfigureAwait(false);

            return response;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress, string templateId)
        {
            Validate();

            var sendEmailResponse = await assessmentApiService.SendEmail(GetSessionId(), domain, emailAddress, templateId).ConfigureAwait(false);

            return sendEmailResponse;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            Validate();

            return await resultsApiService.GetResults(GetSessionId()).ConfigureAwait(false);
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            Validate();

            return await assessmentApiService.FilterAssessment(GetSessionId(), jobCategory).ConfigureAwait(false);
        }

        public async Task<string> Reload(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);

            var assessment = await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);

            persistanceService.SetValue(SessionKey.SessionId, assessment.SessionId);

            return assessment.SessionId;
        }

        private void Validate()
        {
            ValidateSession();
        }

        private void ValidateSession()
        {
            if (!HasSessionId())
            {
                throw new ArgumentException("Session has not been set");
            }
        }

        private string GetSessionId()
        {
            return persistanceService.GetValue(SessionKey.SessionId);
        }

        private bool HasSessionId()
        {
            return !string.IsNullOrWhiteSpace(GetSessionId());
        }

        private DfcUserSession CreateDfcUserSession(string dysacSessionId)
        {
            var dysacSessionIdSegments = dysacSessionId.Split("-", StringSplitOptions.RemoveEmptyEntries);

            var dfcUserSession = new DfcUserSession();
            dfcUserSession.PartitionKey = dysacSessionIdSegments.FirstOrDefault();
            dfcUserSession.SessionId = dysacSessionIdSegments.LastOrDefault();

            return dfcUserSession;
        }
    }
}
