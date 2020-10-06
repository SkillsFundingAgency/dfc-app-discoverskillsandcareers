using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class AssessmentCalculationService : IAssessmentCalculationService
    {
        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService;
        private readonly IMapper mapper;

        private static readonly Dictionary<Answer, int> AnswerMappings = new Dictionary<Answer, int>()
        {
            { Answer.StronglyDisagree, -2 },
            { Answer.Disagree, -1 },
            { Answer.Neutral, 0 },
            { Answer.Agree, 1 },
            { Answer.StronglyAgree, 2 },
        };

        public AssessmentCalculationService(
            IDocumentService<DysacTraitContentModel> traitDocumentService,
            IMapper mapper)
        {
            this.traitDocumentService = traitDocumentService;
            this.mapper = mapper;
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

            var allTraits = await traitDocumentService.GetAllAsync().ConfigureAwait(false);

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
                        Text = allTraits.FirstOrDefault(x => x.Title == g.Key!).Description,
                    };
                })
                .Where(x => x.TotalScore > 0)
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var jobCategoryRelevance = await CalculateJobFamilyRelevance(userTraits, allTraits).ConfigureAwait(false);

            var jobCategories =
                    jobCategoryRelevance
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToArray();

            var resultData = new ResultData()
            {
                Traits = userTraits.Where(x => x.TotalScore > 0).ToArray(),
                JobCategories = jobCategories.ToList(),
                TraitScores = userTraits,
                TraitText = userTraits.Select(x => x.Text)
            };

            assessment.ShortQuestionResult = resultData;

            return assessment;
        }

        public async Task<IEnumerable<JobCategoryResult>> CalculateJobFamilyRelevance(IEnumerable<TraitResult> userTraits, IEnumerable<DysacTraitContentModel> allTraits)
        {
            var results = new List<JobCategoryResult>();

            var topTraits = userTraits.OrderByDescending(x => x.TotalScore).Take(10);

            foreach (var trait in topTraits)
            {
                var applicableTrait = allTraits.FirstOrDefault(x => x.Title == trait.TraitCode);

                if (applicableTrait == null)
                {
                    throw new InvalidOperationException($"Trait {applicableTrait} not found in trait repository");
                }

                results.AddRange(applicableTrait.JobCategories.Select(z =>
                new JobCategoryResult()
                {
                    JobFamilyName = z.Title,
                    JobFamilyText = null,
                    JobFamilyUrl = z.WebsiteURI!.Substring(z.WebsiteURI.LastIndexOf("/") + 1, z.WebsiteURI.Length - z.WebsiteURI.LastIndexOf("/") - 1).ToString(),
                    TraitsTotal = trait.TotalScore,
                    TraitQuestions = z.JobProfiles.SelectMany(x => x.Skills.Select(y => y.Title)).Distinct(),
                    TraitValues = allTraits.Where(x => x.JobCategories.Any(y => y.ItemId == z.ItemId)).Select(p => new TraitValue { TraitCode = p.Title!.ToUpperInvariant(), NormalizedTotal = trait.TotalScore, Total = trait.TotalScore }),
                    NormalizedTotal = trait.TotalScore,
                    Total = trait.TotalScore,
                    TotalQuestions = z.JobProfiles.SelectMany(x => x.Skills.Select(y => y.Title)).Distinct().Count(),
                    NumberOfMatchedJobProfile = z.JobProfiles.Count,
                    JobProfiles = z.JobProfiles.Select(x => mapper.Map<JobProfileResult>(x)),
                }));
            }

            return results.OrderByDescending(t => t.Total).Take(10);
        }
    }
}
