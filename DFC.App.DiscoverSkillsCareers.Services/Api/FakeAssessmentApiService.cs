using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using FluentNHibernate.Conventions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class FakeAssessmentApiService : IAssessmentApiService
    {
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService;
        private readonly IMapper mapper;

        public FakeAssessmentApiService(IDocumentService<DysacAssessment> assessmentDocumentService, IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService, IMapper automapper)
        {
            this.assessmentDocumentService = assessmentDocumentService;
            this.questionSetDocumentService = questionSetDocumentService;
            this.mapper = automapper;
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string sessionId, PostAnswerRequest postAnswerRequest)
        {
            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesmment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();

            var idAsInt = Convert.ToInt32(postAnswerRequest.QuestionId);

            assessment.Questions.FirstOrDefault(x => x.Ordinal == idAsInt).Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)postAnswerRequest.SelectedOption };
            
            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            return new PostAnswerResponse { IsSuccess = true, NextQuestionNumber = assessment.Questions.FirstOrDefault(x => x.Ordinal == idAsInt).Ordinal.Value + 1 };
        }

        public Task<FilterAssessmentResponse> FilterAssessment(string sessionId, string jobCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAssessmentResponse> GetAssessment(string sessionId)
        {
            var assessments = assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId);

            //Return the real next question number here
            await Task.Delay(0).ConfigureAwait(false);

            return new GetAssessmentResponse()
            {
                QuestionSetVersion = "foo",
                SessionId = "session-1",
                CurrentFilterAssessmentCode = "bar",
                CurrentQuestionNumber = 1,
                IsFilterAssessment = false,
                JobCategorySafeUrl = "http://somejobcategory.com",
                MaxQuestionsCount = 1,
                PercentComplete = 0,
                NextQuestionNumber = 1,
                PreviousQuestionNumber = 1,
                QuestionId = "q1",
                QuestionNumber = 1,
                QuestionSetName = "short",
                QuestionText = "Would you like to answer a question?",
                StartedDt = DateTime.Now,
                TraitCode = "LEADER",
                RecordedAnswersCount = 0,
                ReferenceCode = "abc123",
            };
        }

        public async Task<GetQuestionResponse> GetQuestion(string sessionId, string assessmentType, int questionNumber)
        {
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

            var completed = (assessment.Questions.Count(x => x.Answer != null) - assessment.Questions.Count()) / 100;

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

        public async Task<NewSessionResponse> NewSession(string assessmentType)
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

            return new NewSessionResponse
            {
                SessionId = assessmentCode,
            };
        }

        public Task<SendEmailResponse> SendEmail(string sessionId, string domain, string emailAddress, string templateId)
        {
            throw new NotImplementedException();
        }

        public Task<SendSmsResponse> SendSms(string sessionId, string domain, string mobile, string templateId)
        {
            throw new NotImplementedException();
        }
    }
}
