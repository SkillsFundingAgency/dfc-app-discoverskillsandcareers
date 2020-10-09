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
                Questions = questionSet != null && questionSet.Any() ? questionSet.FirstOrDefault().ShortQuestions.OrderBy(z => z.Ordinal).Select(x => mapper.Map<ShortQuestion>(x)) : new List<ShortQuestion>(),
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

            var sessionId = session.State!.SessionId;

            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();
            var question = assessment.Questions.FirstOrDefault(x => (x.Ordinal!.Value + 1) == questionNumber);

            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionNumber} in Assessment {sessionId} not found");
            }

            var completed = (int)((assessment.Questions.Count(x => x.Answer != null) / (decimal)assessment.Questions.Count()) * 100M);
            var currentQuestionNumber = question.Ordinal! + 1;

            return new GetQuestionResponse
            {
                SessionId = sessionId,
                IsComplete = false,
                CurrentQuestionNumber = questionNumber,
                IsFilterAssessment = false,
                MaxQuestionsCount = assessment.Questions.Count(),
                PercentComplete = completed,
                NextQuestionNumber = currentQuestionNumber + 1,
                PreviousQuestionNumber = currentQuestionNumber - 1,
                QuestionId = question.Id!.Value.ToString(),
                QuestionNumber = currentQuestionNumber.Value,
                QuestionText = question.QuestionText!,
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

        public async Task<PostAnswerResponse> AnswerFilterQuestion(string jobCategory, int realQuestionNumber, int questionNumberCounter, int answer)
        {
            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            if (assessment.FilteredAssessment == null)
            {
                throw new InvalidOperationException($"Filtered Assessment for {assessment.AssessmentCode} not found");
            }

            var answeredQuestion = assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.Ordinal == realQuestionNumber);

            if (answeredQuestion == null)
            {
                throw new InvalidOperationException($"Question number {realQuestionNumber} not found in filtered assessment {jobCategory}-{assessment.AssessmentCode}");
            }

            answeredQuestion.Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)answer };

            // TODO: Check result status codes
            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            var answeredTraits = assessment.FilteredAssessment.Questions.Where(x => x.Answer != null).Select(y => y.TraitCode);
            assessment.FilteredAssessment.JobCategoryAssessments.FirstOrDefault(x => x.JobCategory == jobCategory).LastAnswer = DateTime.UtcNow;

            var jobCategoryRequiredTraits = assessment.FilteredAssessment.JobCategoryAssessments.FirstOrDefault(x => x.JobCategory == jobCategory).QuestionSkills.Select(x => x.Key);

            bool completed = true;

            foreach (var trait in jobCategoryRequiredTraits)
            {
                if (!answeredTraits.Contains(trait))
                {
                    completed = false;
                }
            }

            if (completed)
            {
                return new PostAnswerResponse { IsSuccess = true, IsComplete = completed };
            }
            else
            {
                var unasnweredQuestionTraits = jobCategoryRequiredTraits.Where(x => !answeredTraits.Contains(x));
                var nextQuestion = assessment.FilteredAssessment.Questions.Where(x => unasnweredQuestionTraits.Contains(x.TraitCode)).OrderBy(z => z.Ordinal).FirstOrDefault();

                return new PostAnswerResponse { IsSuccess = true, IsFilterAssessment = true, IsComplete = false, NextQuestionNumber = nextQuestion.Ordinal!.Value };
            }
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
                ReferenceCode = sessionId,
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

            if (assessment.FilteredAssessment == null)
            {
                if (assessment.FilteredAssessment == null)
                {
                    assessment.FilteredAssessment = new FilteredAssessment();
                }

                var filterQuestions = await dysacFilteringQuestionService.GetAsync(x => x.PartitionKey == "FilteringQuestion").ConfigureAwait(false);

                int questionNumber = 0;
                foreach (var question in filterQuestions!)
                {
                    question.Ordinal = questionNumber;
                    questionNumber++;
                }

                assessment.FilteredAssessment.Questions = filterQuestions.Select(x => new FilteredAssessmentQuestion { QuestionText = x.Text, Id = x.Id, Ordinal = x.Ordinal, TraitCode = x.Skills.FirstOrDefault().Title });

                foreach (var jobCat in assessment.ShortQuestionResult!.JobCategories!)
                {
                    var applicableQuestions = assessment.FilteredAssessment.Questions.Select(x => new { Code = x.TraitCode, x.Ordinal });

                    assessment.FilteredAssessment.JobCategoryAssessments.Add(new JobCategoryAssessment { JobCategory = jobCat.JobFamilyNameUrl, QuestionSkills = applicableQuestions.Where(x => jobCat.SkillQuestions != null && jobCat.SkillQuestions.Contains(x.Code)).ToDictionary(x => x.Code!, x => x.Ordinal!.Value) });
                }

                await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
            }

            return new FilterAssessmentResponse()
            {
                QuestionNumber = 1,
                SessionId = sessionId,
            };
        }

        public async Task<GetQuestionResponse> GetFilteredAssessmentQuestion(string assessmentType, int questionNumber)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            if (assessment.FilteredAssessment == null)
            {
                throw new InvalidOperationException($"Filtered Assessment for Session {sessionId} not found");
            }

            var jobCategoryAssessment = assessment.FilteredAssessment.JobCategoryAssessments.FirstOrDefault(x => x.JobCategory == assessmentType);
            var categoryQuestions = jobCategoryAssessment.QuestionSkills.OrderBy(x => x.Value).ToList();
            var answeredQuestions = assessment.FilteredAssessment.Questions.Where(x => x.Answer != null).Select(y => y.TraitCode);

            var nextQuestionCode = categoryQuestions.Where(x => !answeredQuestions.Contains(x.Key)).OrderBy(z => z.Value).FirstOrDefault().Key;

            if (questionNumber == 1 && categoryQuestions.All(x => answeredQuestions.Contains(x.Key)))
            {
                // Make sure we reset the questions associated to this assessment in case it's a re-answer
                var jobCategoryAssessmentQuestions = jobCategoryAssessment.QuestionSkills.Select(x => x.Key);

                foreach (var jobCategoryQuestion in jobCategoryAssessmentQuestions)
                {
                    assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.TraitCode == jobCategoryQuestion).Answer = null;
                }

                nextQuestionCode = categoryQuestions[questionNumber - 1].Key;

                await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
            }

            var question = assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.TraitCode == nextQuestionCode);

            return new GetQuestionResponse
            {
                SessionId = sessionId,
                IsComplete = false,
                CurrentQuestionNumber = question.Ordinal!.Value,
                IsFilterAssessment = true,
                NextQuestionNumber = questionNumber + 1,
                PreviousQuestionNumber = questionNumber - 1,
                QuestionId = question.Id!.Value.ToString(),
                QuestionNumber = questionNumber,
                QuestionText = question.QuestionText!,
                StartedDt = DateTime.Now,
                TraitCode = question.TraitCode!,
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
    }
}
