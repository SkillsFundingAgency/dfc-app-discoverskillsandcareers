using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Core.Enums;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsService : IResultsService
    {
        private readonly ISessionService sessionService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService;
        private readonly IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService;

        public ResultsService(
            ISessionService sessionService,
            IAssessmentCalculationService assessmentCalculationService,
            IDocumentService<DysacAssessment> assessmentDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService,
            IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService)
        {
            this.sessionService = sessionService;
            this.assessmentCalculationService = assessmentCalculationService;
            this.assessmentDocumentService = assessmentDocumentService;
            this.filteringQuestionDocumentService = filteringQuestionDocumentService;
            this.jobProfileCategoryDocumentService = jobProfileCategoryDocumentService;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessments = await assessmentDocumentService.GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assessment {sessionId} is null");
            }

            var assessment = assessments.FirstOrDefault();

            return await ProcessAssessment(assessment).ConfigureAwait(false);
        }

        public async Task<GetResultsResponse> GetResultsByCategory(string jobCategoryName)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessments = await assessmentDocumentService.GetAsync(x => x.Id == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assessment {sessionId} is null");
            }

            var assessment = assessments.FirstOrDefault();

            if (assessment.FilteredAssessment == null || assessment.ShortQuestionResult == null)
            {
                throw new InvalidOperationException($"Assessment not in the correct state for results. Short State: {assessment.ShortQuestionResult}, Filtered State: {assessment.FilteredAssessment}, Assessment: {assessment.Id}");
            }

            await UpdateJobCategoryCounts(assessment).ConfigureAwait(false);

            var answeredQuestions = assessment.FilteredAssessment.Questions!
                .Where(question => question.Answer != null)
                .Select(question => (question.TraitCode, question.Answer!.Value))
                .ToList();

            var allFilteringQuestions = await filteringQuestionDocumentService
                .GetAsync(x => x.PartitionKey == "FilteringQuestion").ConfigureAwait(false);

            var questionSkills = allFilteringQuestions?
                .SelectMany(x => x.Skills)
                .Select(x => x.Title)
                .GroupBy(x => x)
                .Select(x => x.First())
                .ToList();

            var allJobCategories =
                await jobProfileCategoryDocumentService.GetAsync(document => document.PartitionKey == "JobProfileCategory")
                    .ConfigureAwait(false);

            var allJobProfiles = allJobCategories!
                .SelectMany(jobCategory => jobCategory.JobProfiles)
                .GroupBy(jobProfile => jobProfile.Title)
                .Select(jobProfileGroup => jobProfileGroup.First())
                .ToList();

            var prominentSkills = JobCategorySkillMappingHelper.CalculateCommonSkillsByPercentage(allJobProfiles);

            foreach (var category in assessment.ShortQuestionResult.JobCategories!)
            {
                var listOfJobProfiles = new List<JobProfileResult>();
                var categoryJobProfiles = category.JobProfiles
                    .Where(jobProfile => jobProfile.SkillCodes != null && jobProfile.SkillCodes.Any())
                    .ToList();

                var categorySkills = JobCategorySkillMappingHelper.GetSkillAttributes(
                    categoryJobProfiles
                        .Select(jobProfile => allJobProfiles.Single(ajp => ajp.Title == jobProfile.Title)),
                    prominentSkills,
                    75).ToList();

                var categoryAnsweredQuestions = categorySkills
                    .Where(categorySkill => answeredQuestions.Any(answeredQuestion => categorySkill.ONetAttribute == answeredQuestion.TraitCode))
                    .Select(categorySkill => (categorySkill.ONetAttribute,
                        answeredQuestions.First(answeredQuestion => categorySkill.ONetAttribute == answeredQuestion.TraitCode).Value))
                    .ToList();

                foreach (var jobProfile in categoryJobProfiles)
                {
                    var fullJobProfile = allJobProfiles.Single(ajp => ajp.Title == jobProfile.Title);

                    var relevantSkills = fullJobProfile
                        .SkillsToCompare(prominentSkills)
                        .Select(s => s.Title)
                        .Where(skillCode => questionSkills!.Contains(skillCode))
                        .Where(skillCode => categorySkills.Any(categorySkill => categorySkill.ONetAttribute == skillCode))
                        .ToList();

                    var profileAnswers = categoryAnsweredQuestions
                        .Select(categoryAnsweredQuestion => (categoryAnsweredQuestion.ONetAttribute,
                            relevantSkills.Contains(categoryAnsweredQuestion.ONetAttribute) ? Answer.Yes : Answer.No))
                        .ToList();

                    var canAddJobProfile = categoryAnsweredQuestions.SequenceEqual(profileAnswers);

                    if (canAddJobProfile)
                    {
                        listOfJobProfiles.Add(jobProfile);
                    }
                }

                assessment.ShortQuestionResult.JobCategories
                    .First(x => x.JobFamilyNameUrl == category.JobFamilyNameUrl).JobProfiles = listOfJobProfiles;
            }

            var jobCategories = OrderResults(assessment.ShortQuestionResult.JobCategories!, jobCategoryName);

            return new GetResultsResponse { JobCategories = jobCategories };
        }

        private async Task UpdateJobCategoryCounts(DysacAssessment assessment)
        {
            var answeredQuestions = assessment.FilteredAssessment!.Questions.Where(z => z.Answer != null).Select(x => x.TraitCode).ToList();

            foreach (var jobCategory in assessment.FilteredAssessment.JobCategoryAssessments)
            {
                var remainingJobCategoryQuestionsCount = jobCategory.QuestionSkills.Count(x => !answeredQuestions.Contains(x.Key));

                if (assessment.ShortQuestionResult != null && assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategory.JobCategory) != null)
                {
                    assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategory.JobCategory).TotalQuestions = remainingJobCategoryQuestionsCount;
                }
            }

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);
        }

        private async Task<GetResultsResponse> ProcessAssessment(DysacAssessment assessment)
        {
            var assessmentCalculationResponse = await assessmentCalculationService.ProcessAssessment(assessment).ConfigureAwait(false);

            if (assessmentCalculationResponse == null)
            {
                throw new InvalidOperationException($"Assessment Caluclation Response is null for {assessment.Id}");
            }

            await assessmentDocumentService.UpsertAsync(assessmentCalculationResponse).ConfigureAwait(false);

            return new GetResultsResponse
            {
                LastAssessmentCategory = assessment.FilteredAssessment?.JobCategoryAssessments
                    .OrderByDescending(x => x.LastAnswer)
                    .FirstOrDefault()?.JobCategory!,
                JobCategories = assessmentCalculationResponse.ShortQuestionResult?.JobCategories!,
                JobFamilyCount = assessmentCalculationResponse.ShortQuestionResult?.JobCategories.Count(),
                Traits = assessmentCalculationResponse.ShortQuestionResult?.TraitText!,
                SessionId = assessment.Id!,
                AssessmentType = "short",
            };
        }

        private IEnumerable<JobCategoryResult> OrderResults(IEnumerable<JobCategoryResult> categories, string selectedCategory)
        {
            int order = categories.Count();
            foreach (var c in categories.OrderByDescending(x => x.JobProfiles.Count()))
            {
                c.DisplayOrder = order;

                if (!string.IsNullOrEmpty(selectedCategory) && c.JobFamilyNameUrl == selectedCategory.ToLower()?.Replace(" ", "-").Replace(",", string.Empty))
                {
                    c.DisplayOrder = 9999;
                }

                order--;
            }

            return categories;
        }
    }
}
