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

        private readonly IDocumentStore documentStore;
        private readonly IMapper mapper;
        private readonly ILogger<AssessmentCalculationService> logger;
        private readonly IAssessmentService assessmentService;
        private readonly IMemoryCache memoryCache;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;

        public AssessmentCalculationService(
            IDocumentStore documentStore,
            IAssessmentService assessmentService,
            IMemoryCache memoryCache,
            IMapper mapper,
            ILoggerFactory loggerFactory,
            ISharedContentRedisInterface sharedContentRedisInterface)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.assessmentService = assessmentService;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
            this.logger = loggerFactory.CreateLogger<AssessmentCalculationService>();
            this.sharedContentRedisInterface = sharedContentRedisInterface;
        }

        public async Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment)
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

            return await RunShortAssessmentCalculation(assessment, allTraits).ConfigureAwait(false);
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

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            var traitLookup = userTraits
                .Where(traitResult => traitResult.TotalScore > 0)
                .ToDictionary(keySelector: traitResult => traitResult.TraitCode, StringComparer.InvariantCultureIgnoreCase);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

            logger.LogInformation("User Traits: {Data}", JsonConvert.SerializeObject(userTraits));
            logger.LogInformation("All Traits: {Data}", JsonConvert.SerializeObject(allTraits));
            logger.LogInformation("Top Traits: {Data}", JsonConvert.SerializeObject(topTraits));
            logger.LogInformation("All Filtering Questions: {Data}", JsonConvert.SerializeObject(allFilteringQuestions));

            var allJobProfiles = allJobProfileCategories
                .SelectMany(jobCategory => jobCategory.JobProfiles)
                .GroupBy(jobProfile => jobProfile.Title)
                .Select(jobProfileGroup => jobProfileGroup.First())
                .ToList();

            var prominentSkills = JobCategorySkillMappingHelper.CalculateCommonSkillsByPercentage(allJobProfiles);

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
                        .First(jobProfileCategory => jobProfileCategory.Title == limitedJobCategory.Title);

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

        public async Task<DysacAssessment> RunShortAssessmentCalculation(DysacAssessment assessment, List<DysacTraitContentModel> allTraits)
        {
            var allFilteringQuestions = await assessmentService.GetFilteringQuestions().ConfigureAwait(false);

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

            var allJobCategories = await JobCategoryHelper.GetJobCategories(sharedContentRedisInterface, mapper).ConfigureAwait(false);

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

        private static IEnumerable<TraitResult> LimitTraits(TraitResult[] traitResult)
        {
            var traitsTake = traitResult.Length > 3 && traitResult[2].TotalScore == traitResult[3].TotalScore ? 4 : 3;
            return traitResult.Take(traitsTake);
        }

        private async Task<List<DysacTraitContentModel>?> GetTraits()
        {
            var traintsResponse = await this.sharedContentRedisInterface.GetDataAsync<PersonalityTraitResponse>("DYSAC/Traits");
            var traits = new List<DysacTraitContentModel>();
            if (traintsResponse != null)
            {
                traits = mapper.Map<List<DysacTraitContentModel>>(source: traintsResponse.PersonalityTraits);
            }

            return traits;
        }
    }
}
