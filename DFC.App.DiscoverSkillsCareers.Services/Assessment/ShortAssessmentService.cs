using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Assessment
{
    public class ShortAssessmentService : IAssessmentService<ShortAssessment>
    {
        private const string MissingAnswerExceptionMessage = "Missing answer";
        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionClient sessionClient;

        public ShortAssessmentService(IAssessmentApiService assessmentApiService, ISessionClient sessionClient)
        {
            this.assessmentApiService = assessmentApiService;
            this.sessionClient = sessionClient;
        }

        public async Task<GetAssessmentResponse> GetAssessmentAsync()
        {
            var sessionId = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            return await assessmentApiService.GetAssessment(sessionId).ConfigureAwait(false);
        }

        public async Task CreateAssessmentAsync(DfcUserSession dfcUserSession = null)
        {
            if (dfcUserSession != null)
            {
                throw new NotImplementedException("Accepting dfc user session is not yet implemented");
            }

            var newSessionResponse = await assessmentApiService.NewSession(AssessmentTypeName.ShortAssessment).ConfigureAwait(false);
            if (newSessionResponse != null)
            {
                var dysacSessionIdSegments = newSessionResponse.SessionId.Split("-", StringSplitOptions.RemoveEmptyEntries);
                var userSession = new DfcUserSession
                {
                    PartitionKey = dysacSessionIdSegments.FirstOrDefault(),
                    SessionId = dysacSessionIdSegments.LastOrDefault(),
                };

                sessionClient.CreateCookie(userSession, false);
            }
        }

        public async Task<GetQuestionResponse> GetQuestionAsync(int questionNumber)
        {
            var sessionId = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            return await assessmentApiService.GetQuestion(sessionId, AssessmentTypeName.ShortAssessment, questionNumber).ConfigureAwait(false);
        }

        public async Task<PostAnswerResponse> AnswerQuestionAsync(int questionNumber, string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException(MissingAnswerExceptionMessage, nameof(answer));
            }

            var questionSetResponse = await GetQuestionAsync(questionNumber).ConfigureAwait(false);
            var questionIdFull = $"{questionSetResponse.QuestionSetVersion}-{questionNumber}";
            var postAnswer = new PostAnswerRequest
            {
                QuestionId = questionIdFull,
                SelectedOption = answer,
            };

            var sessionId = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            var answerResponse = await assessmentApiService.AnswerQuestion(sessionId, postAnswer).ConfigureAwait(false);
            answerResponse.Question = questionSetResponse;

            return answerResponse;
        }
    }
}