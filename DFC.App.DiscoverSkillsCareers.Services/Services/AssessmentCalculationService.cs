﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class AssessmentCalculationService : IAssessmentCalculationService
    {
        private static readonly Dictionary<Answer, int> AnswerMappings = new Dictionary<Answer, int>
        {
            { Answer.StronglyDisagree, -2 },
            { Answer.Disagree, -1 },
            { Answer.Neutral, 0 },
            { Answer.Agree, 1 },
            { Answer.StronglyAgree, 2 },
        };

        private readonly IDocumentStore documentStore;
        private readonly IMapper mapper;
        private readonly ILogger<AssessmentCalculationService> logger;
        private readonly IAssessmentService assessmentService;
        private readonly IMemoryCache memoryCache;

        public AssessmentCalculationService(
            IDocumentStore documentStore,
            IAssessmentService assessmentService,
            IMemoryCache memoryCache,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));

            this.assessmentService = assessmentService;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
            this.logger = loggerFactory.CreateLogger<AssessmentCalculationService>();
        }

        public async Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment)
        {
            return await RunShortAssessmentCalculation(assessment).ConfigureAwait(false);
        }

        public IEnumerable<JobCategoryResult> CalculateJobFamilyRelevance(
            List<TraitResult> userTraits,
            List<DysacTraitContentModel> allTraits,
            List<DysacFilteringQuestionContentModel> allFilteringQuestions,
            List<DysacJobProfileCategoryContentModel> allJobProfileCategories)
        {
            var results = new List<JobCategoryResult>();

            var topTraits = userTraits
                .OrderByDescending(userTrait => userTrait.TotalScore)
                .Take(10);

            var traitLookup = userTraits
                .Where(traitResult => traitResult.TotalScore > 0)
                .ToDictionary(traitResult => traitResult.TraitCode, StringComparer.InvariantCultureIgnoreCase);

            logger.LogInformation("User Traits: {Data}", JsonConvert.SerializeObject(userTraits));
            logger.LogInformation("All Traits: {Data}", JsonConvert.SerializeObject(allTraits));
            logger.LogInformation("Top Traits: {Data}", JsonConvert.SerializeObject(topTraits));
            logger.LogInformation("All Filtering Questions: {Data}", JsonConvert.SerializeObject(allFilteringQuestions));

            var allJobProfiles = allJobProfileCategories
                .SelectMany(jobCategory => jobCategory.JobProfiles)
                .GroupBy(jobProfile => jobProfile.Title)
                .Select(jobProfileGroup => jobProfileGroup.First())
                .ToList();

            var prominentSkills =
                JobCategorySkillMappingHelper.CalculateCommonSkillsByPercentage(allJobProfiles);

            foreach (var trait in topTraits)
            {
                var applicableTrait = allTraits.FirstOrDefault(traitA => traitA.Title == trait.TraitCode);

                if (applicableTrait == null)
                {
                    throw new InvalidOperationException($"Trait {applicableTrait} not found in trait repository");
                }

                foreach (var limitedJobCategory in applicableTrait.JobCategories)
                {
                    var fullJobCategory = allJobProfileCategories
                        .First(jobProfileCategory => jobProfileCategory.Url == limitedJobCategory.Url);

                    var jobCategoryTraits = allTraits
                        .Where(traitA => traitA.JobCategories.Any(jc => jc.Title == limitedJobCategory.Title))
                        .Select(traitA => traitA.Title)
                        .ToList();

                    if (!jobCategoryTraits.All(jobCategoryTrait => traitLookup.ContainsKey(jobCategoryTrait)))
                    {
                        continue;
                    }

                    var jobProfiles = fullJobCategory.JobProfiles
                        .GroupBy(jobProfile => jobProfile.Title)
                        .Select(jobProfileGroup => jobProfileGroup.First())
                        .ToList();

                    var jobProfilesWithAtLeastOneSkill = fullJobCategory.JobProfiles
                        .Where(jobProfile => jobProfile.Skills.Any())
                        .GroupBy(jobProfile => jobProfile.Title)
                        .Select(jobProfileGroup => jobProfileGroup.First())
                        .ToList();

                    var categorySkills = jobProfilesWithAtLeastOneSkill.GetSkillAttributes(
                        prominentSkills,
                        75);

                    logger.LogInformation("Job Category: {Data}", JsonConvert.SerializeObject(fullJobCategory));
                    logger.LogInformation("Category Skills: {Data}", JsonConvert.SerializeObject(categorySkills));

                    if (results.Any(jobCategory => jobCategory.JobFamilyName == fullJobCategory.Title))
                    {
                        var result = results.First(jobCategory => jobCategory.JobFamilyName == fullJobCategory.Title);
                        result.Total += trait.TotalScore;

                        continue;
                    }

                    var skillQuestions = categorySkills
                        .Where(categorySkill =>
                            allFilteringQuestions.Any(applicableQuestion =>
                                 applicableQuestion.Skills.Select(skill => skill.Title).Contains(categorySkill.ONetAttribute)))
                        .Select(skillAttribute => skillAttribute.ONetAttribute!)
                        .ToList();

                    results.Add(new JobCategoryResult
                    {
                        JobFamilyName = fullJobCategory.Title!,
                        JobFamilyUrl = limitedJobCategory.WebsiteURI?.Substring(
                            limitedJobCategory.WebsiteURI.LastIndexOf("/", StringComparison.Ordinal) + 1,
                            limitedJobCategory.WebsiteURI.Length - limitedJobCategory.WebsiteURI.LastIndexOf("/", StringComparison.Ordinal) - 1),
                        SkillQuestions = skillQuestions,
                        TraitValues = allTraits
                            .Where(traitA => traitA.JobCategories.Any(jobCategory => jobCategory.ItemId == fullJobCategory.ItemId))
                            .Select(traitA => new TraitValue
                            {
                                TraitCode = traitA.Title!.ToUpperInvariant(),
                                NormalizedTotal = trait.TotalScore,
                                Total = trait.TotalScore,
                            }).ToList(),
                        Total = trait.TotalScore,
                        TotalQuestions = skillQuestions.Count,
                        JobProfiles = jobProfiles.Select(jobProfile => mapper.Map<JobProfileResult>(jobProfile)),
                    });
                }
            }

            return results.OrderByDescending(jobCategory => jobCategory.Total);
        }

        private static IEnumerable<TraitResult> LimitTraits(TraitResult[] traitResult)
        {
            var traitsTake = traitResult.Length > 3 && traitResult[2].TotalScore == traitResult[3].TotalScore ? 4 : 3;
            return traitResult.Take(traitsTake);
        }

        private async Task<DysacAssessment> RunShortAssessmentCalculation(DysacAssessment assessment)
        {
            if (assessment == null)
            {
                throw new ArgumentNullException(nameof(assessment));
            }

            var allTraits = await GetTraits().ConfigureAwait(false);

            if (allTraits == null)
            {
                throw new InvalidOperationException("No traits retrieved from document service");
            }

            var allFilteringQuestions = await assessmentService.GetFilteringQuestions().ConfigureAwait(false);

            // User traits
            var userTraits = assessment.Questions
                .Select(question => new
                {
                    question.Trait,
                    Score = !question.IsNegative ? AnswerMappings[question.Answer!.Value]
                        : AnswerMappings[question.Answer!.Value] * -1,
                })
                .GroupBy(traitAndScore => traitAndScore.Trait)
                .Select(trait =>
                {
                    return new TraitResult
                    {
                        TraitCode = trait.Key!,
                        TotalScore = trait.Sum(traitAndScore => traitAndScore.Score),
                        Text = allTraits.First(traitA => traitA.Title == trait.Key!).Description!,
                    };
                })
                .Where(traitResult => traitResult.TotalScore > 0)
                .OrderByDescending(traitResult => traitResult.TotalScore)
                .ToList();

            var allJobCategories = await GetJobCategories().ConfigureAwait(false);

            var jobCategoryRelevance = CalculateJobFamilyRelevance(
                userTraits,
                allTraits,
                allFilteringQuestions!,
                allJobCategories!);

            var jobCategories = jobCategoryRelevance
                .OrderByDescending(jobCategoryResult => jobCategoryResult.Total)
                .ThenByDescending(jobCategoryResult => jobCategoryResult.SkillQuestions.Any())
                .Take(10)
                .ToArray();

            var limitedTraits = LimitTraits(
                userTraits
                    .Where(traitResult => traitResult.TotalScore > 0)
                    .ToArray());

            assessment.ShortQuestionResult = new ResultData
            {
                Traits = userTraits,
                JobCategories = jobCategories.ToList(),
                TraitText = limitedTraits.Select(traitResult => traitResult.Text!),
            };

            return assessment;
        }

        private async Task<List<DysacJobProfileCategoryContentModel>?> GetJobCategories()
        {
            if (memoryCache.TryGetValue(nameof(GetJobCategories), out var filteringQuestionsFromCache))
            {
                return (List<DysacJobProfileCategoryContentModel>?)filteringQuestionsFromCache;
            }

            var jobCategories = await documentStore.GetAllContentAsync<DysacJobProfileCategoryContentModel>(
                "JobProfileCategory").ConfigureAwait(false);

            if (!jobCategories?.Any() != true)
            {
                return jobCategories;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
            memoryCache.Set(nameof(GetJobCategories), jobCategories, cacheEntryOptions);

            return jobCategories;
        }

        private async Task<List<DysacTraitContentModel>?> GetTraits()
        {
            if (memoryCache.TryGetValue(nameof(GetTraits), out var filteringQuestionsFromCache))
            {
                return (List<DysacTraitContentModel>?)filteringQuestionsFromCache;
            }

            var traits = await documentStore.GetAllContentAsync<DysacTraitContentModel>(
                "Trait").ConfigureAwait(false);

            if (!traits?.Any() != true)
            {
                return traits;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
            memoryCache.Set(nameof(GetTraits), traits, cacheEntryOptions);

            return traits;
        }
    }
}