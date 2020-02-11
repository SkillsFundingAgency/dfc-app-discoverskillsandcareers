using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ApiService : IApiService
    {
        private const string SessionId = "SessionId";

        private readonly IAssessmentApiService assessmentApiService;
        private readonly IResultsApiService resultsApiService;
        private readonly ISessionService sessionService;

        public ApiService(
            IAssessmentApiService assessmentApiService,
            IResultsApiService resultsApiService,
            ISessionService sessionService)
        {
            this.assessmentApiService = assessmentApiService;
            this.resultsApiService = resultsApiService;
            this.sessionService = sessionService;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var newSessionResponse = await assessmentApiService.NewSession(assessmentType).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                sessionService.SetValue(SessionId, newSessionResponse.SessionId);
            }

            return newSessionResponse != null;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessment, int questionNumber)
        {
            Validate();

            var getQuestionResponse = await assessmentApiService.GetQuestion(GetSessionId(), assessment, questionNumber).ConfigureAwait(false);

            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessment, int questionId, string answer)
        {
            Validate();

            var questionSetResponse = await GetQuestion(assessment, questionId).ConfigureAwait(false);

            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{questionId}";
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
            return sessionService.GetValue<string>(SessionId);
        }

        private bool HasSessionId()
        {
            return !string.IsNullOrWhiteSpace(GetSessionId());
        }
    }
}
