using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ILogger<AssessmentService> logger;
        private readonly NotifyOptions notifyOptions;
        //private readonly IAssessmentApiService assessmentApiService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService;
        private readonly IMapper mapper;

        public AssessmentService(
            ILogger<AssessmentService> logger,
            NotifyOptions notifyOptions,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionService sessionService,
            IDocumentService<DysacAssessment> assessmentDocumentService,
            IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService,
            IMapper mapper)
        {
            this.logger = logger;
            this.notifyOptions = notifyOptions;
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionService = sessionService;
            this.assessmentDocumentService = assessmentDocumentService;
            this.questionSetDocumentService = questionSetDocumentService;
            this.mapper = mapper;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var questionSet = await questionSetDocumentService.GetAsync(x => x.Type == assessmentType.ToLower()).ConfigureAwait(false);
            var assessmentId = Guid.NewGuid();
            var assessmentCode = SessionIdHelper.GenerateSessionId("ncs");

            var assessment = new DysacAssessment()
            {
                Id = assessmentId,
                Questions = questionSet.FirstOrDefault().ShortQuestions.OrderBy(z => z.Ordinal).Select(x => mapper.Map<ShortQuestion>(x)),
                AssessmentCode = assessmentCode,
            };

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            await sessionService.CreateCookie(assessmentCode).ConfigureAwait(false);

            return true;
        }

        public async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var session = await sessionService.GetCurrentSession().ConfigureAwait(false);

            if (session == null)
            {
                throw new InvalidOperationException("Session is null");
            }

            var sessionId = session.State.SessionId;

            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();
            var question = assessment.Questions.FirstOrDefault(x => x.Ordinal == questionNumber);

            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionNumber} in Assessment {sessionId} not found");
            }

            var completed = (int)((assessment.Questions.Count(x => x.Answer != null) / (decimal)assessment.Questions.Count()) * 100M);

            return new GetQuestionResponse
            {
                QuestionSetVersion = "foo",
                SessionId = sessionId,
                CurrentFilterAssessmentCode = "bar",
                IsComplete = false,
                CurrentQuestionNumber = questionNumber,
                IsFilterAssessment = false,
                JobCategorySafeUrl = "http://somejobcategory.com",
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = question.Ordinal + 1,
                PreviousQuestionNumber = question.Ordinal - 1,
                QuestionId = question.Id.Value.ToString(),
                QuestionNumber = question.Ordinal.Value,
                QuestionSetName = "short",
                QuestionText = question.QuestionText,
                StartedDt = DateTime.Now,
                TraitCode = "LEADER",
                RecordedAnswersCount = assessment.Questions.Count(x => x.Answer != null),
            };
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, int answer)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesmment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();

            var idAsInt = Convert.ToInt32(realQuestionNumber);

            assessment.Questions.FirstOrDefault(x => x.Ordinal == idAsInt).Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)answer };

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            return new PostAnswerResponse { IsSuccess = true, NextQuestionNumber = assessment.Questions.FirstOrDefault(x => x.Ordinal == idAsInt).Ordinal.Value + 1 };
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();

            var question = assessment.Questions.OrderBy(x => x.Ordinal).FirstOrDefault(z => z.Answer == null);

            var completed = (int)((assessment.Questions.Count(x => x.Answer != null) / (decimal)assessment.Questions.Count()) * 100M);

            //Return the real next question number here
            return new GetAssessmentResponse
            {
                QuestionSetVersion = "foo",
                SessionId = sessionId,
                CurrentFilterAssessmentCode = "bar",
                CurrentQuestionNumber = question.Ordinal.Value,
                IsFilterAssessment = false,
                JobCategorySafeUrl = "http://somejobcategory.com",
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = question.Ordinal + 1,
                PreviousQuestionNumber = question.Ordinal - 1,
                QuestionId = question.Id.Value.ToString(),
                QuestionNumber = question.Ordinal.Value,
                QuestionSetName = "short",
                QuestionText = question.QuestionText,
                StartedDt = DateTime.Now,
                TraitCode = "LEADER",
                RecordedAnswersCount = assessment.Questions.Count(x => x.Answer != null),
            };
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<SendSmsResponse> SendSms(string domain, string mobile)
        {
            throw new NotImplementedException();
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ReloadUsingReferenceCode(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            return await ReloadUsingSessionId(sessionId).ConfigureAwait(false);
        }

        public async Task<bool> ReloadUsingSessionId(string sessionId)
        {
            throw new NotImplementedException();
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
    }
}
