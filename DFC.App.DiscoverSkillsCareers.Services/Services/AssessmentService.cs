using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
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
        private readonly INotificationService notificationService;

        public AssessmentService(
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionService sessionService,
            IDocumentService<DysacAssessment> assessmentDocumentService,
            IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionService,
            IMapper mapper,
            INotificationService notificationService)
        {
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionService = sessionService;
            this.assessmentDocumentService = assessmentDocumentService;
            this.questionSetDocumentService = questionSetDocumentService;
            this.dysacFilteringQuestionService = dysacFilteringQuestionService;
            this.mapper = mapper;
            this.notificationService = notificationService;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var questionSet = await questionSetDocumentService.GetAsync(x => x.PartitionKey == "QuestionSet").ConfigureAwait(false);
            var assessmentCode = SessionIdHelper.GenerateSessionId("ncs");

            var assessment = new DysacAssessment()
            {
                StartedAt = DateTime.UtcNow,
                Questions = questionSet != null && questionSet.Any() ? questionSet.FirstOrDefault().ShortQuestions.OrderBy(z => z.Ordinal).Select(x => mapper.Map<ShortQuestion>(x)) : new List<ShortQuestion>(),
                Id = assessmentCode,
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

            var assessments = await assessmentDocumentService.GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

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
                NextQuestionNumber = questionNumber + 1,
                PreviousQuestionNumber = questionNumber - 1,
                QuestionId = question.Id!.Value.ToString(),
                QuestionNumber = currentQuestionNumber.Value,
                QuestionText = question.QuestionText!,
                StartedDt = DateTime.Now,
                RecordedAnswersCount = assessment.Questions.Count(x => x.Answer != null),
            };
        }

        public async Task UpdateQuestionNumber(int questionNumber)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = await assessmentDocumentService.GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesmment {sessionId} not found");
            }

            var assessment = assessments.FirstOrDefault();
            var questions = assessment.Questions.ToList();

            for (int idx = questionNumber - 1, len = questions.Count; idx < len; idx++)
            {
                questions[idx].Answer = null;
            }

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, int answer)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = await assessmentDocumentService.GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

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
                throw new InvalidOperationException($"Filtered Assessment for {assessment.Id} not found");
            }

            var answeredQuestion = assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.Ordinal == realQuestionNumber);

            if (answeredQuestion == null)
            {
                throw new InvalidOperationException($"Question number {realQuestionNumber} not found in filtered assessment {jobCategory}-{assessment.Id}");
            }

            answeredQuestion.Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)answer };

            var answeredTraits = assessment.FilteredAssessment
                .Questions!
                .Where(x => x.Answer != null)
                .Select(y => y.TraitCode);

            assessment.FilteredAssessment.JobCategoryAssessments
                .FirstOrDefault(x => x.JobCategory == jobCategory) !.LastAnswer = DateTime.UtcNow;

            var jobCategoryRequiredTraits = assessment.FilteredAssessment.JobCategoryAssessments
                .FirstOrDefault(x => x.JobCategory == jobCategory) !
                .QuestionSkills
                .Select(x => x.Key);

            var completed = true;

            foreach (var trait in jobCategoryRequiredTraits)
            {
                if (!answeredTraits.Contains(trait))
                {
                    completed = false;
                }
            }

            assessment.FilteredAssessment.CurrentFilterAssessmentCode = completed ? null : jobCategory;
            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

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

            var questionNumber = question != null ? question.Ordinal!.Value : 0;
            var atLeastOneAnsweredFilterQuestion =
                assessment.FilteredAssessment?.Questions?.Any(q => q.Answer != null) == true;

            var jobCategoryRequiredTraits = assessment.FilteredAssessment?.JobCategoryAssessments
                .FirstOrDefault(trait => trait.JobCategory == assessment.FilteredAssessment?.CurrentFilterAssessmentCode) !
                .QuestionSkills
                .Select(questionSkill => questionSkill.Key)
                .ToList();

            var categoryQuestions = assessment.FilteredAssessment?.Questions!.Where(
                categoryQuestion => jobCategoryRequiredTraits?.Contains(categoryQuestion.TraitCode!) == true).ToList();

            var allFilteringQuestionsForCategoryAnswered =
                !string.IsNullOrEmpty(assessment.FilteredAssessment?.CurrentFilterAssessmentCode) &&
                categoryQuestions?.All(x => x.Answer != null) == true;

            return new GetAssessmentResponse
            {
                SessionId = sessionId,
                CurrentFilterAssessmentCode = assessment.FilteredAssessment?.CurrentFilterAssessmentCode,
                CurrentQuestionNumber = questionNumber + 1,
                IsFilterAssessment = assessment.Questions.All(x => x != null)
                    && assessment.ShortQuestionResult != null
                    && assessment.FilteredAssessment != null
                    && atLeastOneAnsweredFilterQuestion,
                JobCategorySafeUrl = string.Empty,
                MaxQuestionsCount = assessment.Questions.Count(),
                QuestionId = question != null ? question.Id!.Value.ToString() : string.Empty,
                QuestionNumber = questionNumber + 1,
                QuestionText = question != null ? question.QuestionText! : string.Empty,
                StartedDt = assessment.StartedAt,
                RecordedAnswersCount = assessment.Questions.Count(x => x.Answer != null),
                ReferenceCode = sessionId,
                AtLeastOneAnsweredFilterQuestion = atLeastOneAnsweredFilterQuestion,
                AllFilteringQuestionsForCategoryAnswered = allFilteringQuestionsForCategoryAnswered,
            };
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService.SendEmail(domain, emailAddress, SessionHelper.FormatSessionId(sessionId));
        }

        public async Task<SendSmsResponse> SendSms(string domain, string mobile)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService.SendSms(domain, mobile, SessionHelper.FormatSessionId(sessionId));
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

                var filterQuestions = await dysacFilteringQuestionService
                    .GetAsync(x => x.PartitionKey == "FilteringQuestion").ConfigureAwait(false);

                assessment.FilteredAssessment.Questions = filterQuestions
                    .Select(x => new FilteredAssessmentQuestion
                    {
                        QuestionText = x.Text,
                        Id = x.Id,
                        Ordinal = x.Ordinal,
                        TraitCode = x.Skills.FirstOrDefault()?.Title,
                    });

                foreach (var jobCat in assessment.ShortQuestionResult!.JobCategories!)
                {
                    var applicableQuestions = assessment.FilteredAssessment.Questions
                        .Select(x => new
                        {
                            Code = x.TraitCode,
                            x.Ordinal,
                        });

                    var questionSkills = jobCat.SkillQuestions
                        .Select(skillQuestion =>
                            applicableQuestions.SingleOrDefault(applicableQuestion => skillQuestion == applicableQuestion.Code))
                        .Where(applicableQuestion => applicableQuestion != null)
                        .ToDictionary(applicableQuestion => applicableQuestion!.Code!, x => x!.Ordinal!.Value);

                    assessment.FilteredAssessment.JobCategoryAssessments.Add(
                        new JobCategoryAssessment
                        {
                            JobCategory = jobCat.JobFamilyNameUrl,
                            QuestionSkills = questionSkills,
                        });
                }

                await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
            }

            return new FilterAssessmentResponse
            {
                QuestionNumber = 1,
                SessionId = sessionId,
            };
        }

        public async Task<GetQuestionResponse> GetFilteredAssessmentQuestion(string jobCategory, int questionNumber)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            if (assessment.FilteredAssessment == null)
            {
                throw new InvalidOperationException($"Filtered Assessment for Session {sessionId} not found");
            }

            var jobCategoryAssessment = assessment.FilteredAssessment.JobCategoryAssessments.FirstOrDefault(x => x.JobCategory == jobCategory);
            var categoryQuestions = jobCategoryAssessment.QuestionSkills.ToList();
            var answeredQuestions = assessment.FilteredAssessment.Questions.Where(x => x.Answer != null).Select(y => y.TraitCode);

            var nextQuestionCode = categoryQuestions.FirstOrDefault(x => !answeredQuestions.Contains(x.Key)).Key;

            if (questionNumber == 1 && categoryQuestions.All(x => answeredQuestions.Contains(x.Key)))
            {
                // Make sure we reset the questions associated to this assessment in case it's a re-answer
                var jobCategoryAssessmentQuestions = jobCategoryAssessment.QuestionSkills.Select(x => x.Key);

                foreach (var jobCategoryQuestion in jobCategoryAssessmentQuestions)
                {
                    assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.TraitCode == jobCategoryQuestion).Answer = null;
                }

                nextQuestionCode = categoryQuestions.Count >= questionNumber ? categoryQuestions[questionNumber - 1].Key : string.Empty;

                await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
            }

            var question = assessment.FilteredAssessment.Questions.FirstOrDefault(x => x.TraitCode == nextQuestionCode);

            if (question == null)
            {
                throw new Exception($"Cannot find next question by trait ({nextQuestionCode})");
            }

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
            var assessments = await assessmentDocumentService
                .GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                return false;
            }

            await sessionService.CreateCookie(sessionId).ConfigureAwait(false);

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

        private async Task<DysacAssessment> GetCurrentAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            var assessments = (await assessmentDocumentService
                .GetAsync(x => x.Id == sessionId).ConfigureAwait(false)) !.ToList();

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assesment {sessionId} not found");
            }

            var assessment = assessments.First();
            const int questionSetCount = 40;

            if (assessment.Questions.Count() < questionSetCount)
            {
                var questionSets = await questionSetDocumentService
                    .GetAsync(document => document.PartitionKey == "QuestionSet").ConfigureAwait(false);

                var missingQuestions = questionSets!
                    .First()
                    .ShortQuestions!
                    .Where(shortQuestion => !assessment.Questions.Any(assessmentQuestion => assessmentQuestion.Ordinal == shortQuestion.Ordinal));

                var shortQuestions = new List<ShortQuestion>(assessment.Questions);
                shortQuestions.AddRange(missingQuestions.Select(missingQuestion => new ShortQuestion
                {
                    Id = missingQuestion.ItemId,
                    QuestionText = missingQuestion.Title,
                    IsNegative = missingQuestion.Impact!.ToUpperInvariant() != "POSITIVE",
                    Ordinal = missingQuestion.Ordinal,
                    Answer = null,
                    Trait = missingQuestion.Traits.First().Title,
                }));

                if (shortQuestions.Count > assessment.Questions.Count())
                {
                    assessment.Questions = shortQuestions;
                    await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
                }
            }

            return assessment;
        }
    }
}
