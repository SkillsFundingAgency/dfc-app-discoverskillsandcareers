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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionService;
        private readonly IMapper mapper;

        public AssessmentService(
            ILogger<AssessmentService> logger,
            NotifyOptions notifyOptions,
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionService sessionService,
            IDocumentService<DysacAssessment> assessmentDocumentService,
            IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionService,
            IMapper mapper)
        {
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionService = sessionService;
            this.assessmentDocumentService = assessmentDocumentService;
            this.questionSetDocumentService = questionSetDocumentService;
            this.dysacFilteringQuestionService = dysacFilteringQuestionService;
            this.mapper = mapper;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var questionSet = await questionSetDocumentService.GetAsync(x => x.Type == assessmentType.ToLower()).ConfigureAwait(false);
            var assessmentId = Guid.NewGuid();
            var assessmentCode = SessionIdHelper.GenerateSessionId("ncs");

            var assessment = new DysacAssessment()
            {
                StartedAt = DateTime.UtcNow,
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
            var question = assessment.Questions.FirstOrDefault(x => (x.Ordinal.Value + 1) == questionNumber);

            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionNumber} in Assessment {sessionId} not found");
            }

            var completed = (int)((assessment.Questions.Count(x => x.Answer != null) / (decimal)assessment.Questions.Count()) * 100M);
            var currentQuestionNumber = question.Ordinal + 1;

            return new GetQuestionResponse
            {
                SessionId = sessionId,
                IsComplete = false,
                CurrentQuestionNumber = questionNumber,
                IsFilterAssessment = false,
                JobCategorySafeUrl = "http://somejobcategory.com",
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = currentQuestionNumber + 1,
                PreviousQuestionNumber = currentQuestionNumber - 1,
                QuestionId = question.Id.Value.ToString(),
                QuestionNumber = currentQuestionNumber.Value,
                QuestionText = question.QuestionText,
                StartedDt = DateTime.Now,
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

            assessment.Questions.FirstOrDefault(x => (x.Ordinal + 1) == idAsInt).Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)answer };

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            return new PostAnswerResponse { IsSuccess = true, NextQuestionNumber = realQuestionNumber + 1, IsComplete = assessment.Questions.All(x => x.Answer != null) };
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            var question = assessment.Questions.OrderBy(x => x.Ordinal).FirstOrDefault(z => z.Answer == null);

            var completed = (int)((assessment.Questions.Count(x => x.Answer != null) / (decimal)assessment.Questions.Count()) * 100M);

            var currentQuestionNumber = question != null ? question.Ordinal + 1 : 0;

            return new GetAssessmentResponse
            {
                SessionId = sessionId,
                CurrentFilterAssessmentCode = string.Empty,
                CurrentQuestionNumber = currentQuestionNumber!.Value + 1,
                IsFilterAssessment = assessment.Questions.All(x => x != null) && assessment.ShortQuestionResult != null,
                JobCategorySafeUrl = string.Empty,
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = currentQuestionNumber.Value + 1,
                PreviousQuestionNumber = currentQuestionNumber.Value - 1,
                QuestionId = question != null ? question.Id!.Value.ToString() : string.Empty,
                QuestionNumber = currentQuestionNumber.Value,
                QuestionText = question != null ? question.QuestionText! : string.Empty,
                StartedDt = assessment.StartedAt,
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
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessment = await GetCurrentAssessment().ConfigureAwait(false);
            var filterQuestionsToLoad = assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategory).TraitQuestions;

            var filterQuestions = await dysacFilteringQuestionService.GetAsync(x => x.PartitionKey == "FilteringQuestion" && filterQuestionsToLoad.Contains(x.Title)).ConfigureAwait(false);


            int questionNumber = 0;
            foreach (var question in filterQuestions!)
            {
                question.Ordinal = questionNumber;
                questionNumber++;
            }

            assessment.FilteredAssessments.Add(new FilteredAssessment { JobCategory = jobCategory, AssessmentFilteredQuestions = filterQuestions.Select(x => new FilteredAssessmentQuestion { QuestionText = x.Text, Id = x.Id, Ordinal = x.Ordinal }).ToList() });

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            return new FilterAssessmentResponse()
            {
                QuestionNumber = 1,
                SessionId = sessionId,
            };
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

        private async Task<DysacAssessment> GetCurrentAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();

            return assessment;
        }

        public async Task<GetQuestionResponse> GetFilteredAssessmentQuestion(string assessmentType, int questionNumber)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            var filteredAssessment = assessment.FilteredAssessments.FirstOrDefault(x => x.JobCategory == assessmentType);

            var question = filteredAssessment.AssessmentFilteredQuestions.FirstOrDefault(x => x.Ordinal + 1 == questionNumber);

            var completed = (int)((filteredAssessment.AssessmentFilteredQuestions.Count(x => x.Answer != null) / (decimal)filteredAssessment.AssessmentFilteredQuestions.Count()) * 100M);

            return new GetQuestionResponse
            {
                SessionId = sessionId,
                IsComplete = false,
                CurrentQuestionNumber = questionNumber,
                IsFilterAssessment = false,
                JobCategorySafeUrl = "http://somejobcategory.com",
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = questionNumber + 1,
                PreviousQuestionNumber = questionNumber - 1,
                QuestionId = question.Id.Value.ToString(),
                QuestionNumber = questionNumber,
                QuestionText = question.QuestionText,
                StartedDt = DateTime.Now,
                TraitCode = "LEADER",
                RecordedAnswersCount = assessment.Questions.Count(x => x.Answer != null),
            };
        }
    }
}
