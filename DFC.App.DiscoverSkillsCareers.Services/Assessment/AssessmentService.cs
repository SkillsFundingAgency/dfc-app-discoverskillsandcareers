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
    public class AssessmentService : IAssessmentService<ShortAssessment>
    {
        private const string AssessmentType = AssessmentTypeName.ShortAssessment;

        private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionClient sessionClient;

        public AssessmentService(IAssessmentApiService assessmentApiService, ISessionClient sessionClient)
        {
            this.assessmentApiService = assessmentApiService;
            this.sessionClient = sessionClient;
        }

        public Task<PostAnswerResponse> AnswerQuestion(int questionNumber, string answer)
        {
            throw new System.NotImplementedException();
        }

        public async Task<DfcUserSession> CreateAssessment()
        {
            DfcUserSession result = null;
            var newSessionResponse = await assessmentApiService.NewSession(AssessmentType).ConfigureAwait(false);

            if (newSessionResponse != null)
            {
                result = CreateDfcUserSession(newSessionResponse.SessionId);

                sessionClient.CreateCookie(result, false);
            }

            return result;
        }

        public Task<GetQuestionResponse> GetQuestion(int questionNumber)
        {
            throw new System.NotImplementedException();
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
