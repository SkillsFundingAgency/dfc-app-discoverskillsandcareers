using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class AssessmentService : IAssessmentService
    {
        private const string HttpContextAssessmentKey = "DysacAssessment";

        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;
        private readonly IDocumentStore documentStore;
        private readonly IMapper mapper;
        private readonly INotificationService notificationService;
        private readonly IHttpContextAccessor accessor;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;

        public AssessmentService(
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionService sessionService,
            IDocumentStore documentStore,
            IMapper mapper,
            INotificationService notificationService,
            IHttpContextAccessor accessor,
            ISharedContentRedisInterface sharedContentRedisInterface)
        {
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionService = sessionService;
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.accessor = accessor;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
        }

        public async Task<bool> NewSession(string assessmentType)
        {
            var questionSets = await GetQuestionSets().ConfigureAwait(false);

            var assessmentCode = SessionIdHelper.GenerateSessionId("ncs");
            var assessment = new DysacAssessment
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                StartedAt = DateTime.UtcNow,
                Questions = questionSets?.Any() == true ?
                    questionSets
                        .First()
                        .ShortQuestions!
                        .OrderBy(shortQuestion => shortQuestion.Ordinal)
                        .Select(shortQuestion => mapper.Map<ShortQuestion>(shortQuestion)) : new List<ShortQuestion>(),
                Id = assessmentCode,
            };

            await UpdateAssessment(assessment).ConfigureAwait(false);
            await sessionService.CreateDysacSession(assessmentCode).ConfigureAwait(false);

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
            var assessment = await GetAssessment(sessionId).ConfigureAwait(false);

            var question = assessment.Questions.FirstOrDefault(question => question.Ordinal!.Value + 1 == questionNumber);

            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionNumber} in Assessment {sessionId} not found");
            }

            var completed = (int)(assessment.Questions.Count(questionA => questionA.Answer != null)
                / (decimal)assessment.Questions.Count() * 100M);

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
                RecordedAnswer = question.Answer,
                RecordedAnswersCount = assessment.Questions.Count(questionA => questionA.Answer != null),
            };
        }

        public async Task<PostAnswerResponse> AnswerQuestion(string assessmentType, int realQuestionNumber, int questionNumberCounter, int answer)
        {
            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            assessment.Questions.First(questionA => questionA.Ordinal + 1 == realQuestionNumber).Answer = new QuestionAnswer
            {
                AnsweredAt = DateTime.UtcNow,
                Value = (Answer)answer,
            };

            await UpdateAssessment(assessment).ConfigureAwait(false);

            return new PostAnswerResponse
            {
                IsSuccess = true,
                NextQuestionNumber = realQuestionNumber + 1,
                IsComplete = assessment.Questions.All(questionA => questionA.Answer != null),
            };
        }

        public async Task<PostAnswerResponse> AnswerFilterQuestion(string jobCategory, int realQuestionNumber, int questionNumberCounter, int answer)
        {
            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            if (assessment.FilteredAssessment == null)
            {
                throw new InvalidOperationException($"Filtered Assessment for {assessment.Id} not found");
            }

            var answeredQuestion = assessment.FilteredAssessment.Questions!
                .FirstOrDefault(question => question.Ordinal == realQuestionNumber);

            if (answeredQuestion == null)
            {
                throw new InvalidOperationException(
                    $"Question number {realQuestionNumber} not found in filtered assessment {jobCategory}-{assessment.Id}");
            }

            answeredQuestion.Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = (Answer)answer };

            var answeredTraits = assessment.FilteredAssessment
                .Questions!
                .Where(question => question.Answer != null)
                .Select(question => question.TraitCode)
                .ToList();

            assessment.FilteredAssessment.JobCategoryAssessments
                .FirstOrDefault(jobCategoryAssessment => jobCategoryAssessment.JobCategory == jobCategory)!.LastAnswer = DateTime.UtcNow;

            var jobCategoryRequiredTraits = assessment.FilteredAssessment.JobCategoryAssessments
                .FirstOrDefault(jobCategoryAssessment => jobCategoryAssessment.JobCategory == jobCategory)!
                .QuestionSkills
                .Select(questionSkill => questionSkill.Key)
                .ToList();

            var completed = jobCategoryRequiredTraits
                .All(trait => answeredTraits.Contains(trait));

            assessment.FilteredAssessment.CurrentFilterAssessmentCode = jobCategory;
            await UpdateAssessment(assessment).ConfigureAwait(false);

            if (completed)
            {
                return new PostAnswerResponse { IsSuccess = true, IsComplete = completed };
            }

            var unansweredQuestionTraits = jobCategoryRequiredTraits
                .Where(jobCategoryRequiredTrait => !answeredTraits.Contains(jobCategoryRequiredTrait));

            var nextQuestion = assessment.FilteredAssessment.Questions!
                .Where(question => unansweredQuestionTraits.Contains(question.TraitCode))
                .OrderBy(question => question.Ordinal)
                .First();

            return new PostAnswerResponse
            {
                IsSuccess = true,
                IsFilterAssessment = true,
                IsComplete = false,
                NextQuestionNumber = nextQuestion.Ordinal!.Value,
            };
        }

        public async Task<GetAssessmentResponse> GetAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            var question = assessment.Questions
                .OrderBy(questionA => questionA.Ordinal)
                .FirstOrDefault(questionA => questionA.Answer == null);

            var questionNumber = question != null ? question.Ordinal!.Value : 0;
            var atLeastOneAnsweredFilterQuestion =
                assessment.FilteredAssessment?.Questions?.Any(q => q.Answer != null) == true;

            var jobCategoryRequiredTraits = assessment.FilteredAssessment?.JobCategoryAssessments
                .FirstOrDefault(trait => trait.JobCategory == assessment.FilteredAssessment?.CurrentFilterAssessmentCode)?
                .QuestionSkills
                .Select(questionSkill => questionSkill.Key)
                .ToList();

            var categoryQuestions = assessment.FilteredAssessment?.Questions?.Where(
                categoryQuestion => jobCategoryRequiredTraits?.Contains(categoryQuestion.TraitCode!) == true).ToList();

            var allFilteringQuestionsForCategoryAnswered =
                !string.IsNullOrEmpty(assessment.FilteredAssessment?.CurrentFilterAssessmentCode) &&
                categoryQuestions?.All(categoryQuestion => categoryQuestion.Answer != null) == true;

            return new GetAssessmentResponse
            {
                SessionId = sessionId,
                CurrentFilterAssessmentCode = assessment.FilteredAssessment?.CurrentFilterAssessmentCode,
                CurrentQuestionNumber = questionNumber + 1,
                IsFilterAssessment = assessment.Questions.All(questionA => questionA != null)
                    && assessment.ShortQuestionResult != null
                    && assessment.FilteredAssessment != null
                    && atLeastOneAnsweredFilterQuestion,
                JobCategorySafeUrl = string.Empty,
                MaxQuestionsCount = assessment.Questions.Count(),
                QuestionId = question != null ? question.Id!.Value.ToString() : string.Empty,
                QuestionNumber = questionNumber + 1,
                QuestionText = question != null ? question.QuestionText! : string.Empty,
                StartedDt = assessment.StartedAt,
                RecordedAnswersCount = assessment.Questions.Count(questionA => questionA.Answer != null),
                ReferenceCode = sessionId,
                AtLeastOneAnsweredFilterQuestion = atLeastOneAnsweredFilterQuestion,
                AllFilteringQuestionsForCategoryAnswered = allFilteringQuestionsForCategoryAnswered,
            };
        }

        public async Task<DysacAssessment> GetAssessment(string sessionId)
        {
            return (await GetAssessment(sessionId, true).ConfigureAwait(false))!;
        }

        public async Task<DysacAssessment?> GetAssessment(string sessionId, bool throwErrorWhenNotFound)
        {

            if (accessor.HttpContext != null && accessor.HttpContext.Items.ContainsKey(HttpContextAssessmentKey))
            {
              return (DysacAssessment?)accessor.HttpContext.Items[HttpContextAssessmentKey];
            }
            
            var assessment = await documentStore.GetAssessmentAsync(sessionId)
                .ConfigureAwait(false);

            if (assessment != null && accessor.HttpContext != null)
            {
                accessor.HttpContext.Items.Add(HttpContextAssessmentKey, assessment);
                return assessment;
            }

            if (!throwErrorWhenNotFound)
            {
                return null;
            }

            throw new InvalidOperationException($"Assessment {sessionId} not found");
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService
                .SendEmail(domain, emailAddress, sessionId, SessionHelper.FormatSessionId(sessionId));
        }

        public async Task<SendSmsResponse> SendSms(string domain, string mobile)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService
                .SendSms(domain, mobile, sessionId, SessionHelper.FormatSessionId(sessionId));
        }

        public async Task<FilterAssessmentResponse> FilterAssessment(string jobCategory)
        {
            var assessment = await GetCurrentAssessment().ConfigureAwait(false);

            if (assessment.FilteredAssessment == null)
            {
                var filterQuestions = await GetFilteringQuestions().ConfigureAwait(false);

                assessment.FilteredAssessment = new FilteredAssessment
                {
                    Questions = filterQuestions!
                        .Select(filterQuestion => new FilteredAssessmentQuestion
                        {
                            QuestionText = filterQuestion.Text,
                            Id = filterQuestion.Id,
                            Ordinal = filterQuestion.Ordinal,
                            TraitCode = filterQuestion.Skills.FirstOrDefault()?.Title,
                        }),
                };

                var filteredAssessmentQuestions = assessment.FilteredAssessment.Questions.ToList();

                foreach (var jobCat in assessment.ShortQuestionResult!.JobCategories!)
                {
                    var applicableQuestions = filteredAssessmentQuestions
                        .Select(filteredAssessmentQuestion => new
                        {
                            Code = filteredAssessmentQuestion.TraitCode,
                            filteredAssessmentQuestion.Ordinal,
                        });

                    var questionSkills = jobCat.SkillQuestions
                        .Select(skillQuestion =>
                            applicableQuestions.SingleOrDefault(applicableQuestion => skillQuestion == applicableQuestion.Code))
                        .Where(applicableQuestion => applicableQuestion != null)
                        .ToDictionary(applicableQuestion => applicableQuestion!.Code!, ordinal => ordinal!.Ordinal!.Value);

                    assessment.FilteredAssessment.JobCategoryAssessments.Add(
                        new JobCategoryAssessment
                        {
                            JobCategory = jobCat.JobFamilyNameUrl,
                            QuestionSkills = questionSkills,
                        });
                }

                await UpdateAssessment(assessment).ConfigureAwait(false);
            }

            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

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

            var jobCategoryAssessment = assessment.FilteredAssessment.JobCategoryAssessments
                .First(jobCategoryAssessment => jobCategoryAssessment.JobCategory == jobCategory);

            var categoryQuestions = jobCategoryAssessment.QuestionSkills.ToList();
            var answeredQuestions = assessment.FilteredAssessment.Questions!
                .Where(question => question.Answer != null)
                .Select(question => question.TraitCode);

            var nextQuestionCode = categoryQuestions[questionNumber - 1].Key;

            if (questionNumber == 1 && categoryQuestions.All(categoryQuestion => answeredQuestions.Contains(categoryQuestion.Key)))
            {
                // Make sure we reset the questions associated to this assessment in case it's a re-answer
                var jobCategoryAssessmentQuestions = jobCategoryAssessment.QuestionSkills
                    .Select(questionSkill => questionSkill.Key);

                foreach (var jobCategoryQuestion in jobCategoryAssessmentQuestions)
                {
                    assessment.FilteredAssessment.Questions!
                        .First(question => question.TraitCode == jobCategoryQuestion).Answer = null;
                }

                nextQuestionCode = categoryQuestions.Count >= questionNumber ? categoryQuestions[questionNumber - 1].Key : string.Empty;

                await UpdateAssessment(assessment).ConfigureAwait(false);
            }

            var question = assessment.FilteredAssessment.Questions!
                .FirstOrDefault(question => question.TraitCode == nextQuestionCode);

            if (question == null)
            {
                throw new KeyNotFoundException($"Cannot find next question by trait ({nextQuestionCode})");
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
                RecordedAnswer = question.Answer,
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
            var assessment = await GetAssessment(sessionId, false).ConfigureAwait(false);

            if (assessment == null)
            {
                return false;
            }

            await sessionService.CreateDysacSession(sessionId).ConfigureAwait(false);

            return true;
        }

        public bool ReferenceCodeExists(string referenceCode)
        {
            var sessionId = sessionIdToCodeConverter.GetSessionId(referenceCode);
            return !string.IsNullOrWhiteSpace(sessionId);
        }

        public async Task UpdateAssessment(DysacAssessment assessment)
        {
            await documentStore.UpdateAssessmentAsync(assessment).ConfigureAwait(false);

            if (accessor.HttpContext != null)
            {
                accessor.HttpContext.Items[HttpContextAssessmentKey] = assessment;
            }
        }

        public async Task<List<DysacFilteringQuestionContentModel>?> GetFilteringQuestions()
        {
            var filteringQuestionResponse = await this.sharedContentRedisInterface.GetDataAsync<PersonalityFilteringQuestionResponse>("DYSAC/FilteringQuestions");
            var filteringQuestions = new List<DysacFilteringQuestionContentModel>();
            if (filteringQuestionResponse != null)
            {
                filteringQuestions = mapper.Map<List<DysacFilteringQuestionContentModel>>(source: filteringQuestionResponse.PersonalityFilteringQuestion);
            }

            return filteringQuestions;
        }

        private async Task<List<DysacQuestionSetContentModel>?> GetQuestionSets()
        {
            var questionSetsResponse = await this.sharedContentRedisInterface.GetDataAsync<PersonalityQuestionSet>("DYSAC/QuestionSets");

            var questionSets = new List<DysacQuestionSetContentModel>();
            var qs = mapper.Map<DysacQuestionSetContentModel>(questionSetsResponse);
            questionSets.Add(qs);

            return questionSets;
        }

        private async Task<DysacAssessment> GetCurrentAssessment()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessment = await GetAssessment(sessionId).ConfigureAwait(false);

            const int questionSetCount = 40;

            if (assessment.Questions.Count() < questionSetCount)
            {
                // Fixing a problem where some are not migrated from the old system.
                await AddMissingQuestions_Legacy(assessment).ConfigureAwait(false);
            }

            return assessment;
        }

        private async Task AddMissingQuestions_Legacy(DysacAssessment assessment)
        {
            var questionSets = await GetQuestionSets().ConfigureAwait(false);

            if (questionSets?.Any() != true)
            {
                return;
            }

            var missingQuestions = questionSets
                .First()
                .ShortQuestions!
                .Where(shortQuestion => assessment.Questions.All(assessmentQuestion => assessmentQuestion.Ordinal != shortQuestion.Ordinal));

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
                await UpdateAssessment(assessment).ConfigureAwait(false);
            }
        }
    }
}
