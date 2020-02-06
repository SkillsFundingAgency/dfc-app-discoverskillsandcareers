using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ApiService : IApiService
    {
        private const string SessionId = "SessionId";

        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionService sessionService;

        public ApiService(IAssessmentApiService assessmentApiService, ISessionService sessionService)
        {
            this.assessmentApiService = assessmentApiService;
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
            PopulateData(getQuestionResponse);

            return getQuestionResponse;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessment, int questionId, string answer)
        {
            Validate();

            var questionSetResponse = await GetQuestion(assessment, questionId).ConfigureAwait(false);

            var questionIdFull = $"{questionSetResponse.QuestionSetName}-{questionId}";
            var post = new PostAnswerRequest() { QuestionId = questionIdFull, SelectedOption = answer };
            var answerQuestionResponse = await assessmentApiService.AnswerQuestion(GetSessionId(), post).ConfigureAwait(false);

            return answerQuestionResponse;
        }

        private void PopulateData(GetQuestionResponse getQuestionResponse)
        {
            if (getQuestionResponse != null)
            {
                var lastDashIndex = getQuestionResponse.QuestionId.LastIndexOf("-");
                if (lastDashIndex != -1)
                {
                    getQuestionResponse.CurrentQuestionNumber = int.Parse(getQuestionResponse.QuestionId.Substring(lastDashIndex + 1));
                    getQuestionResponse.QuestionSetName = getQuestionResponse.QuestionId.Substring(0, lastDashIndex);
                    getQuestionResponse.PreviousQuestionNumber = getQuestionResponse.CurrentQuestionNumber;
                    if (getQuestionResponse.PreviousQuestionNumber < 0)
                    {
                        getQuestionResponse.PreviousQuestionNumber = 0;
                    }
                }
            }
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
