using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService;
        private readonly IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService;
        private readonly IMapper mapper;
        private readonly ILogger<AssessmentCalculationService> logger;
        private readonly IAssessmentService assessmentService;
        private readonly IMemoryCache memoryCache;

        public AssessmentCalculationService(
            IDocumentService<DysacTraitContentModel> traitDocumentService,
            IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService,
            IAssessmentService assessmentService,
            IMemoryCache memoryCache,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            this.traitDocumentService = traitDocumentService;
            this.jobProfileCategoryDocumentService = jobProfileCategoryDocumentService;
            this.assessmentService = assessmentService;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
            this.logger = loggerFactory.CreateLogger<AssessmentCalculationService>();
        }

        public async Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment)
        {
            var result = await RunShortAssessmentCalculation(assessment).ConfigureAwait(false);
            return result;
        }

        public IEnumerable<JobCategoryResult> CalculateJobFamilyRelevance(
            List<TraitResult> userTraits,
            List<DysacTraitContentModel> allTraits,
            List<DysacFilteringQuestionContentModel> allFilteringQuestions,
            List<DysacJobProfileCategoryContentModel> allJobProfileCategories)
        {
            var results = new List<JobCategoryResult>();

            var topTraits = userTraits.OrderByDescending(userTrait => userTrait.TotalScore).Take(10);

            var traitLookup = userTraits.Where(r => r.TotalScore > 0)
                .ToDictionary(r => r.TraitCode, StringComparer.InvariantCultureIgnoreCase);

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
                var applicableTrait = allTraits.FirstOrDefault(x => x.Title == trait.TraitCode);

                if (applicableTrait == null)
                {
                    throw new InvalidOperationException($"Trait {applicableTrait} not found in trait repository");
                }

                foreach (var limitedJobCategory in applicableTrait.JobCategories)
                {
                    var fullJobCategory = allJobProfileCategories
                        .First(x => x.Url == limitedJobCategory.Url);

                    var jobCategoryTraits = allTraits
                        .Where(tr => tr.JobCategories.Any(jc => jc.Title == limitedJobCategory.Title))
                        .Select(tr => tr.Title)
                        .ToList();

                    if (!jobCategoryTraits.All(tc => traitLookup.ContainsKey(tc)))
                    {
                        continue;
                    }

                    var jobProfiles = fullJobCategory.JobProfiles
                        .GroupBy(jp => jp.Title)
                        .Select(jpg => jpg.First())
                        .ToList();

                    var jobProfilesWithAtLeastOneSkill = fullJobCategory.JobProfiles
                        .Where(z => z.Skills.Any())
                        .GroupBy(jp => jp.Title)
                        .Select(jpg => jpg.First())
                        .ToList();

                    var categorySkills = jobProfilesWithAtLeastOneSkill.GetSkillAttributes(
                        prominentSkills,
                        75);

                    logger.LogInformation("Job Category: {Data}", JsonConvert.SerializeObject(fullJobCategory));
                    logger.LogInformation("Category Skills: {Data}", JsonConvert.SerializeObject(categorySkills));

                    if (results.Any(x => x.JobFamilyName == fullJobCategory.Title))
                    {
                        var result = results.First(x => x.JobFamilyName == fullJobCategory.Title);
                        result.Total += trait.TotalScore;

                        continue;
                    }

                    var skillQuestions = categorySkills
                        .Where(categorySkill =>
                            allFilteringQuestions.Any(applicableQuestion =>
                                 applicableQuestion.Skills.Select(x => x.Title).Contains(categorySkill.ONetAttribute)))
                        .Select(z => z.ONetAttribute!)
                        .ToList();

                    results.Add(new JobCategoryResult
                    {
                        JobFamilyName = fullJobCategory.Title!,
                        JobFamilyUrl = limitedJobCategory.WebsiteURI?.Substring(
                            limitedJobCategory.WebsiteURI.LastIndexOf("/", StringComparison.Ordinal) + 1,
                            limitedJobCategory.WebsiteURI.Length - limitedJobCategory.WebsiteURI.LastIndexOf("/", StringComparison.Ordinal) - 1),
                        SkillQuestions = skillQuestions,
                        TraitValues = allTraits
                            .Where(x => x.JobCategories.Any(y => y.ItemId == fullJobCategory.ItemId))
                            .Select(p => new TraitValue
                            {
                                TraitCode = p.Title!.ToUpperInvariant(),
                                NormalizedTotal = trait.TotalScore,
                                Total = trait.TotalScore,
                            }).ToList(),
                        Total = trait.TotalScore,
                        TotalQuestions = skillQuestions.Count,
                        JobProfiles = jobProfiles.Select(x => mapper.Map<JobProfileResult>(x)),
                    });
                }
            }

            return results.OrderByDescending(t => t.Total);
        }

        private static IEnumerable<TraitResult> LimitTraits(TraitResult[] traitResult)
        {
            int traitsTake = traitResult.Length > 3 && traitResult[2].TotalScore == traitResult[3].TotalScore ? 4 : 3;
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
                .Select(x => new
                {
                    x.Trait,
                    Score = !x.IsNegative ? AnswerMappings[x.Answer!.Value]
                        : AnswerMappings[x.Answer!.Value] * -1,
                })
                .GroupBy(x => x.Trait)
                .Select(g =>
                {
                    return new TraitResult
                    {
                        TraitCode = g.Key!,
                        TotalScore = g.Sum(x => x.Score),
                        Text = allTraits.First(x => x.Title == g.Key!).Description!,
                    };
                })
                .Where(x => x.TotalScore > 0)
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var allJobCategories = await GetJobCategories().ConfigureAwait(false);

            var jobCategoryRelevance = CalculateJobFamilyRelevance(
                userTraits, allTraits, allFilteringQuestions!, allJobCategories!);

            var jobCategories = jobCategoryRelevance
                .OrderByDescending(x => x.Total)
                .ThenByDescending(x => x.SkillQuestions.Any())
                .Take(10)
                .ToArray();

            var limitedTraits = LimitTraits(userTraits.Where(x => x.TotalScore > 0).ToArray());

            assessment.ShortQuestionResult = new ResultData
            {
                Traits = userTraits,
                JobCategories = jobCategories.ToList(),
                TraitText = limitedTraits.Select(x => x.Text!),
            };

            return assessment;
        }

        private async Task<List<DysacJobProfileCategoryContentModel>?> GetJobCategories()
        {
            if (memoryCache.TryGetValue(nameof(GetJobCategories), out var filteringQuestionsFromCache))
            {
                return (List<DysacJobProfileCategoryContentModel>?)filteringQuestionsFromCache;
            }

            var jobCategories = (await jobProfileCategoryDocumentService.GetAsync(
                    document => document.PartitionKey == "JobProfileCategory")
                .ConfigureAwait(false))?.ToList();

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

            var traits = (await traitDocumentService.GetAsync(
                document => document.PartitionKey == "Trait")
                .ConfigureAwait(false))?.ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
            memoryCache.Set(nameof(GetTraits), traits, cacheEntryOptions);

            return traits;
        }
    }
}