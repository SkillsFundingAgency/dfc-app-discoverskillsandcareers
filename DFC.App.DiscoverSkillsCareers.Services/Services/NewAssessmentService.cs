using AutoMapper;
using Dfc.DiscoverSkillsAndCareers.Models;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class NewAssessmentService : IAssessmentService
    {
        private readonly ILogger<NewAssessmentService> log;
        private readonly ISessionService sessionService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService;
        private readonly IMapper mapper;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;

        public NewAssessmentService(ILogger<NewAssessmentService> logger, IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService, ISessionService sessionService, IMapper mapper, ISessionIdToCodeConverter sessionIdToCodeConverter)
        {
            this.log = logger;
            this.sessionService = sessionService;
            this.questionSetDocumentService = questionSetDocumentService;
            this.mapper = mapper;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
        }

        public Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, string answer)
        {
            throw new NotImplementedException();
        }

        public Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            //var session = await sessionService.GetCurrentSession().ConfigureAwait(false);

            //if (session == null || session.State!.Questions == null)
            //{
            //    throw new InvalidOperationException($"Session state returned null for user session");
            //}

            //var question = QuestionHelpers.GetNextQuestion(session.State.Questions);

            //if (question == null)
            //{
            //    throw new InvalidOperationException($"Next question for assessment {session.State.SessionId} is null");
            //}

            //var response = new GetAssessmentResponse()
            //{
            //    CurrentFilterAssessmentCode = session.State.FilteredAssessmentState?.CurrentFilterAssessmentCode,
            //    NextQuestionNumber = QuestionHelpers.GetNextQuestionNumber(session.State.Questions),
            //    QuestionId = question!.Id.ToString(),
            //    QuestionText = question!.Text,
            //    TraitCode = question.TraitCode,
            //    QuestionNumber = question.Ordinal.Value,
            //    SessionId = session.State.PrimaryKey,
            //    PercentComplete = QuestionHelpers.GetPercentComplete(session.State.Questions),
            //    ReloadCode = session.State.UserSessionId,
            //    MaxQuestionsCount = session.State.Questions.Count(),
            //    RecordedAnswersCount = session.State.RecordedAnswers.Count(),
            //    StartedDt = session.State.StartedDt,
            //    IsFilterAssessment = session.State.IsFilterAssessment,
            //    JobCategorySafeUrl = (session.State.CurrentState as FilteredAssessmentState)?.JobFamilyNameUrlSafe
            //};

            //return response;

            return new GetAssessmentResponse();
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            //var session = await sessionService.GetCurrentSession().ConfigureAwait(false);

            //if (session == null)
            //{
            //    throw new InvalidOperationException($"Session state returned null for user session");
            //}

            //var question = session.State!.Questions.FirstOrDefault(x => x.Ordinal == questionNumber);

            //if (question == null)
            //{
            //    throw new InvalidOperationException($"Question {questionNumber} not found for session {session.Id}");
            //}

            //// Create mapper for this
            //return mapper.Map<GetQuestionResponse>(question);

            return new GetQuestionResponse();
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            //// Get the current question set version for this assesssment type and title (supplied by CMS - configured in appsettings)
            //var currentQuestionSetInfo = await questionSetDocumentService.GetAsync(x => x.Type == assessmentType).ConfigureAwait(false);

            //if (currentQuestionSetInfo == null)
            //{
            //    log.LogInformation($"Unable to load latest question set {assessmentType}");
            //    return false;
            //}

            //// Create a new user session
            //string sessionId = SessionIdHelper.GenerateSessionId("ncs");
            //string partitionKey = PartitionKeyGenerator.UserSession(sessionId);

            //var userSession = new UserSession()
            //{
            //    PartitionKey = partitionKey,
            //    UserSessionId = sessionId,
            //    //Todo, get from somewhere else
            //    Salt = "ncs",
            //    StartedDt = DateTime.Now,
            //    LanguageCode = "en",
            //    AssessmentState = new AssessmentState("todo", currentQuestionSetInfo.FirstOrDefault().ShortQuestions.Count),
            //    AssessmentType = currentQuestionSetInfo.FirstOrDefault().Type.ToLower(),
            //    Questions = currentQuestionSetInfo.FirstOrDefault().ShortQuestions.OrderBy(x => x.Ordinal).Select(x => mapper.Map<ShortQuestion>(x))
            //};

            //await sessionService.SaveSession(userSession).ConfigureAwait(false);

            //log.LogInformation($"Finished creating new assessment {userSession.UserSessionId}");

            return true;
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

        public Task<bool> ReloadUsingReferenceCode(string referenceCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReloadUsingSessionId(string sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<SendSmsResponse> SendSms(string domain, string mobile)
        {
            throw new NotImplementedException();
        }
    }
}
