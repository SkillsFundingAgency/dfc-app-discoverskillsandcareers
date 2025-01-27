using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class ResultsService : IResultsService
    {
        private const string ExpiryAppSettings = "Cms:Expiry";
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly IDocumentStore documentStore;
        private readonly IMemoryCache memoryCache;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private string status;
        private double expiryInHours = 4;

        public ResultsService(
            ISessionService sessionService,
            IAssessmentService assessmentService,
            IAssessmentCalculationService assessmentCalculationService,
            IDocumentStore documentStore,
            IMemoryCache memoryCache,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.sessionService = sessionService;
            this.assessmentService = assessmentService;
            this.assessmentCalculationService = assessmentCalculationService;

            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.memoryCache = memoryCache;

            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.mapper = mapper;
            this.configuration = configuration;
            status = configuration?.GetSection("contentMode:contentMode").Get<string>();

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            if (this.configuration != null)
            {
                string expiryAppString = this.configuration.GetSection(ExpiryAppSettings).Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }

        }

        public async Task<GetResultsResponse> GetResults(bool updateCollection)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessment = await assessmentService.GetAssessment(sessionId).ConfigureAwait(false);

            return await ProcessAssessment(assessment, updateCollection).ConfigureAwait(false);
        }

        public async Task<GetResultsResponse> GetResultsByCategory(string jobCategoryName)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessment = await assessmentService.GetAssessment(sessionId).ConfigureAwait(false);

            if (assessment.FilteredAssessment == null || assessment.ShortQuestionResult == null)
            {
                throw new InvalidOperationException(
                    $"Assessment not in the correct state for results. Short State: {assessment.ShortQuestionResult}, " +
                    $"Filtered State: {assessment.FilteredAssessment}, Assessment: {assessment.Id}");
            }

            await UpdateJobCategoryCounts(assessment).ConfigureAwait(false);

            var answeredQuestions = assessment.FilteredAssessment.Questions!
                .Where(question => question.Answer != null)
                .Select(question => (question.TraitCode, question.Answer!.Value))
                .ToList();

            var allFilteringQuestions = await assessmentService.GetFilteringQuestions().ConfigureAwait(false);

            var questionSkills = allFilteringQuestions!
                .SelectMany(filteringQuestion => filteringQuestion.Skills)
                .Select(skill => skill.Title)
                .GroupBy(skill => skill)
                .Select(skillGroup => skillGroup.First())
                .ToList();

            var allJobCategories = await JobCategoryHelper.GetJobCategories(sharedContentRedisInterface, mapper, configuration).ConfigureAwait(false);
            var allTrait = await GetTraits().ConfigureAwait(false);

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


                var notMatchingJobProfiles = categoryJobProfiles
                    .Where(jobProfile => allJobProfiles.All(ajp => ajp.Title != jobProfile.Title));

                if (notMatchingJobProfiles.Any())
                {
                    return new GetResultsResponse { AllJobProfilesMatchWithAssessmentProfiles = false };
                }

                var categorySkills = categoryJobProfiles
                    .Select(jobProfile => allJobProfiles.Single(ajp => ajp.Title == jobProfile.Title))
                    .ToList()
                    .GetSkillAttributes(
                        prominentSkills,
                        75).ToList();

                var categoryAnsweredQuestions = categorySkills
                    .Where(categorySkill =>
                        answeredQuestions.Any(answeredQuestion => categorySkill.ONetAttribute == answeredQuestion.TraitCode))
                    .Select(categorySkill => (categorySkill.ONetAttribute,
                        answeredQuestions.First(answeredQuestion => categorySkill.ONetAttribute == answeredQuestion.TraitCode).Value))
                    .ToList();

                foreach (var jobProfile in categoryJobProfiles)
                {
                    var fullJobProfile = allJobProfiles.Single(ajp => ajp.Title == jobProfile.Title);

                    var relevantSkills = fullJobProfile
                        .SkillsToCompare(prominentSkills)
                        .Select(s => s.Title)
                        .Where(skillCode => questionSkills.Contains(skillCode))
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
                    .First(jobCategoryResult => jobCategoryResult.JobFamilyNameUrl == category.JobFamilyNameUrl)
                    .JobProfiles = listOfJobProfiles;
            }

            var jobCategories = assessment.ShortQuestionResult.JobCategories!.ToList();

            var jobCategory = jobCategories?.Find(j => Equals(j.JobFamilyNameUrl, jobCategoryName));
            if (jobCategory != null && jobCategory.TotalQuestions == 0)
            {
                jobCategories = OrderResultsByJobCategory(jobCategories, jobCategoryName);
            }

            UpdateJobCategory(jobCategories, allJobCategories);
            UpdateTrait(allTrait, assessment.ShortQuestionResult?.LimitedTraits);
            return new GetResultsResponse { JobCategories = jobCategories, Traits = assessment.ShortQuestionResult?.LimitedTraits! };
        }

        private void UpdateTrait(List<DysacTraitContentModel>? allTrait, IEnumerable<TraitResult> limitedTraits)
        {
            if (limitedTraits != null)
            {
                foreach (var traits in limitedTraits)
                {
                    var selectedTrait = allTrait?.SingleOrDefault(x => x.Title == traits.TraitCode);

                    if (selectedTrait != null)
                    {
                        traits.Title = selectedTrait.Title;
                        traits.Text = selectedTrait.Description;
                        traits.ImagePath = selectedTrait.ImagePath;
                    }
                }
            }
        }

        private void UpdateJobCategory(List<JobCategoryResult> jobCategories, List<DysacJobProfileCategoryContentModel> allJobCategories)
        {
            if (jobCategories != null)
            {
                foreach (var jobCategory in jobCategories)
                {
                    var jc = allJobCategories?.SingleOrDefault(x => x.Title == jobCategory.JobFamilyName);

                    if (jc != null)
                    {
                        jobCategory.JobFamilyText = jc.JobFamilyText;
                        jobCategory.ImagePathDesktop = jc.ImagePathDesktop;
                        jobCategory.ImagePathMobile = jc.ImagePathMobile;
                        jobCategory.ImagePathTitle = jc.ImagePathTitle;
                    }
                }
            }
        }

        private void UpdateCategoryImagePath(List<JobCategoryResult> jobCategories, List<DysacJobProfileCategoryContentModel> allJobCategories)
        {
            foreach (var jobCategory in jobCategories)
            {
                var jc = allJobCategories.SingleOrDefault(x => x.Title == jobCategory.JobFamilyName);

                if (jc != null)
                {
                    jobCategory.JobFamilyText = jc.JobFamilyText;
                    jobCategory.ImagePathDesktop = jc.ImagePathDesktop;
                    jobCategory.ImagePathMobile = jc.ImagePathMobile;
                    jobCategory.ImagePathTitle = jc.ImagePathTitle;
                }
            }
        }

        private async Task<List<DysacTraitContentModel>?> GetTraits()
        {
            var traintsResponse = await this.sharedContentRedisInterface.GetDataAsyncWithExpiry<PersonalityTraitResponse>(Constants.DYSACPersonalityTrait, status, expiryInHours);
            var traits = new List<DysacTraitContentModel>();
            if (traintsResponse != null)
            {
                traits = mapper.Map<List<DysacTraitContentModel>>(source: traintsResponse.PersonalityTraits);
            }

            return traits;
        }

        private async Task UpdateJobCategoryCounts(DysacAssessment assessment)
        {
            var answeredQuestions = assessment.FilteredAssessment!.Questions!
                .Where(filteredAssessmentQuestion => filteredAssessmentQuestion.Answer != null)
                .Select(filteredAssessmentQuestion => filteredAssessmentQuestion.TraitCode)
                .ToList();

            foreach (var jobCategory in assessment.FilteredAssessment.JobCategoryAssessments)
            {
                var remainingJobCategoryQuestionsCount = jobCategory.QuestionSkills
                    .Count(questionSkill => !answeredQuestions.Contains(questionSkill.Key));

                if (assessment.ShortQuestionResult != null && assessment.ShortQuestionResult.JobCategories!
                    .FirstOrDefault(jobCategoryResult => jobCategoryResult.JobFamilyNameUrl == jobCategory.JobCategory) != null)
                {
                    assessment.ShortQuestionResult.JobCategories!.
                        First(jobCategoryResult => jobCategoryResult.JobFamilyNameUrl == jobCategory.JobCategory)
                        .TotalQuestions = remainingJobCategoryQuestionsCount;
                }
            }

            await assessmentService.UpdateAssessment(assessment).ConfigureAwait(false);
        }

        private async Task<GetResultsResponse> ProcessAssessment(DysacAssessment assessment, bool updateCollection)
        {
            var assessmentCalculationResponse = await assessmentCalculationService.ProcessAssessment(assessment).ConfigureAwait(false);

            if (assessmentCalculationResponse == null)
            {
                throw new InvalidOperationException($"Assessment Calculation Response is null for {assessment.Id}");
            }

            if (updateCollection)
            {
                await assessmentService.UpdateAssessment(assessmentCalculationResponse).ConfigureAwait(false);
            }

            return new GetResultsResponse
            {
                LastAssessmentCategory = assessment.FilteredAssessment?.JobCategoryAssessments?.Where(t => t.LastAnswer != DateTime.MinValue)
                    .OrderByDescending(jobCategoryAssessment => jobCategoryAssessment.LastAnswer)
                    .FirstOrDefault()?
                    .JobCategory!,
                JobCategories = assessmentCalculationResponse.ShortQuestionResult?.JobCategories,
                JobFamilyCount = assessmentCalculationResponse.ShortQuestionResult?.JobCategories?.Count(),
                Traits = assessmentCalculationResponse.ShortQuestionResult?.LimitedTraits!,
                SessionId = assessment.Id!,
                AssessmentType = "short",
            };
        }

        private List<JobCategoryResult> OrderResultsByJobCategory(List<JobCategoryResult> categories, string selectedCategory)
        {
            var order = categories.Count;

            foreach (var c in categories.OrderByDescending(jobCategoryResult => jobCategoryResult.JobProfiles.Count()))
            {
                c.DisplayOrder = order;

                if (!string.IsNullOrEmpty(selectedCategory)
                    && c.JobFamilyNameUrl == selectedCategory
                        .ToLower()
                        .Replace(" ", "-")
                        .Replace(",", string.Empty))
                {
                    c.DisplayOrder = 9999;
                }

                order--;
            }

            return categories;
        }
    }
}
