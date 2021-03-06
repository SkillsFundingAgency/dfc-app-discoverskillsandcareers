﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Compui.Cosmos.Contracts;
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
        private static readonly Dictionary<Answer, int> AnswerMappings = new Dictionary<Answer, int>()
        {
            { Answer.StronglyDisagree, -2 },
            { Answer.Disagree, -1 },
            { Answer.Neutral, 0 },
            { Answer.Agree, 1 },
            { Answer.StronglyAgree, 2 },
        };

        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService;
        private readonly IMapper mapper;
        private readonly ILogger<AssessmentCalculationService> logger;

        public AssessmentCalculationService(
            IDocumentService<DysacTraitContentModel> traitDocumentService,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            this.traitDocumentService = traitDocumentService;
            this.mapper = mapper;
            this.logger = loggerFactory.CreateLogger<AssessmentCalculationService>();
        }

        public async Task<DysacAssessment> ProcessAssessment(DysacAssessment assessment)
        {
            var result = await RunShortAssessmentCalculation(assessment).ConfigureAwait(false);
            return result;
        }

        public async Task<DysacAssessment> RunShortAssessmentCalculation(DysacAssessment assessment)
        {
            if (assessment == null)
            {
                throw new ArgumentNullException(nameof(assessment));
            }

            var allTraits = await traitDocumentService.GetAsync(x => x.PartitionKey == "Trait").ConfigureAwait(false);

            if (allTraits == null)
            {
                throw new InvalidOperationException("No traits retrieved from document service");
            }

            // User traits
            var userTraits = assessment.Questions
                .Select(x => new
                {
                    x.Trait,
                    Score = !x.IsNegative ? AnswerMappings[x.Answer!.Value!]
                        : AnswerMappings[x.Answer!.Value!] * -1,
                })
                .GroupBy(x => x.Trait)
                .Select(g =>
                {
                    return new TraitResult()
                    {
                        TraitCode = g.Key!,
                        TotalScore = g.Sum(x => x.Score),
                        Text = allTraits.FirstOrDefault(x => x.Title == g.Key!).Description!,
                    };
                })
                .Where(x => x.TotalScore > 0)
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var jobCategoryRelevance = CalculateJobFamilyRelevance(userTraits, allTraits);

            var jobCategories =
                    jobCategoryRelevance
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToArray();

            var limitedTraits = LimitTraits(userTraits.Where(x => x.TotalScore > 0).ToArray());

            var resultData = new ResultData()
            {
                Traits = limitedTraits,
                JobCategories = jobCategories.ToList(),
                TraitText = limitedTraits.Select(x => x.Text!),
            };

            assessment.ShortQuestionResult = resultData;

            return assessment;
        }

        public IEnumerable<JobCategoryResult> CalculateJobFamilyRelevance(IEnumerable<TraitResult> userTraits, IEnumerable<DysacTraitContentModel> allTraits)
        {
            var results = new List<JobCategoryResult>();

            var topTraits = userTraits.OrderByDescending(x => x.TotalScore).Take(10);

            logger.LogInformation($"User Traits:{JsonConvert.SerializeObject(userTraits)}");
            logger.LogInformation($"All Traits:{JsonConvert.SerializeObject(allTraits)}");
            logger.LogInformation($"Top Traits:{JsonConvert.SerializeObject(topTraits)}");

            foreach (var trait in topTraits)
            {
                var applicableTrait = allTraits.FirstOrDefault(x => x.Title == trait.TraitCode);

                if (applicableTrait == null)
                {
                    throw new InvalidOperationException($"Trait {applicableTrait} not found in trait repository");
                }

                foreach (var jc in applicableTrait.JobCategories)
                {
                    if (!results.Any(x => x.JobFamilyName == jc.Title))
                    {
                        var categorySkills = JobCategorySkillMappingHelper.GetSkillAttributes(jc.JobProfiles.Where(z => z.Skills.Any()), new HashSet<string>(), 0.75);

                        logger.LogInformation($"Job Cateogry:{JsonConvert.SerializeObject(jc)}");
                        logger.LogInformation($"Category Skills: {JsonConvert.SerializeObject(categorySkills)}");

                        results.Add(new JobCategoryResult()
                        {
                            JobFamilyName = jc.Title!,
                            JobFamilyUrl = jc.WebsiteURI?.Substring(jc.WebsiteURI.LastIndexOf("/") + 1, jc.WebsiteURI.Length - jc.WebsiteURI.LastIndexOf("/") - 1).ToString(),
                            TraitsTotal = trait.TotalScore,
                            SkillQuestions = categorySkills.Select(z => z.ONetAttribute!),
                            TraitValues = allTraits.Where(x => x.JobCategories.Any(y => y.ItemId == jc.ItemId)).Select(p => new TraitValue { TraitCode = p.Title!.ToUpperInvariant(), NormalizedTotal = trait.TotalScore, Total = trait.TotalScore }).ToList(),
                            NormalizedTotal = trait.TotalScore,
                            Total = trait.TotalScore,
                            TotalQuestions = categorySkills.Count(),
                            JobProfiles = jc.JobProfiles.Select(x => mapper.Map<JobProfileResult>(x)),
                        });
                    }
                }
            }

            return results.OrderByDescending(t => t.Total).Take(10);
        }

        private static IEnumerable<TraitResult> LimitTraits(TraitResult[] traitResult)
        {
            int traitsTake = (traitResult.Length > 3 && traitResult[2].TotalScore == traitResult[3].TotalScore) ? 4 : 3;
            return traitResult.Take(traitsTake);
        }
    }
}