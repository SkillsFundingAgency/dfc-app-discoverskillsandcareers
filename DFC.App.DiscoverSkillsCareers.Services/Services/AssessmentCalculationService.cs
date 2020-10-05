using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class AssessmentCalculationService : IAssessmentCalculationService
    {
        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionDocumentService;

        public AssessmentCalculationService(
            IDocumentService<DysacTraitContentModel> traitDocumentService,
            IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService)
        {
            this.traitDocumentService = traitDocumentService;
            this.questionDocumentService = questionSetDocumentService;
        }

        public async Task CalculateAssessment(DysacAssessment assessment)
        {
            await RunShortAssessment(assessment).ConfigureAwait(false);
            //PrepareFilterAssessmentState(questionSet.QuestionSetVersion, assessment, jobFamilies, questions);

        }

        //public void PrepareFilterAssessmentState(string questionSetVersion, UserSession userSession, JobCategory[] jobFamilies,
        //    Question[] questions)
        //{
        //    if (userSession.ResultData != null)
        //    {
        //        foreach (var jobCategory in userSession.ResultData.JobCategories)
        //        {
        //            var jobFamily = jobFamilies.First(jf => jf.Code.EqualsIgnoreCase(jobCategory.JobCategoryCode));
        //            userSession.FilteredAssessmentState =
        //                userSession.FilteredAssessmentState ?? new FilteredAssessmentState();
        //            userSession.FilteredAssessmentState.CreateOrResetCategoryState(questionSetVersion, questions,
        //                jobFamily);
        //        }
        //    }
        //}

        public async Task RunShortAssessment(DysacAssessment assessment)
        {
            // User traits
            var userTraits = assessment.Questions
                .Select(x => new
                {
                    x.Trait,
                    Score = !x.IsNegative ? (int)x.Answer!.Value
                        : (int)x.Answer!.Value * -1,
                })
                .GroupBy(x => x.Trait)
                .Select(g =>
                {
                    return new TraitResult()
                    {
                        TraitCode = g.Key!,
                        TotalScore = g.Sum(x => x.Score),
                    };
                })
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var jobCategoryRelevance = await CalculateJobFamilyRelevance(userTraits);

            var jobCategories =
                    jobCategoryRelevance
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToArray();


            //var resultData = new ResultData()
            //{
            //    Traits = userTraits.Where(x => x.TotalScore > 0).ToArray(),
            //    JobCategories = jobCategories,
            //    TraitScores = userTraits.ToArray()
            //};

            //userSession.ResultData = resultData;
        }

        public async Task<IEnumerable<JobCategoryResult>> CalculateJobFamilyRelevance(IEnumerable<TraitResult> userTraits)
        {
            var results = new List<JobCategoryResult>();

            var allTraits = await traitDocumentService.GetAllAsync();
            var topTraits = userTraits.OrderByDescending(x => x.TotalScore).Take(10);

            foreach (var trait in topTraits)
            {
                var applicableTrait = allTraits.FirstOrDefault(x => x.Title == trait.TraitCode);

                if (applicableTrait == null)
                {
                    throw new InvalidOperationException($"Trait {applicableTrait} not found in trait repository");
                }

                var traitQuestions = applicableTrait.JobCategories.SelectMany(z => z.JobProfiles.SelectMany(y => y.Skills.Select(k => k.Title))).Distinct();

                results.AddRange(applicableTrait.JobCategories.Select(z =>
                new JobCategoryResult()
                {
                    JobCategoryName = z.Title,
                    JobCategoryText = null,
                    Url = z.Url.ToString(),
                    TraitsTotal = allTraits.Count(x => x.JobCategories.Any(y => y.ItemId == z.ItemId)),
                    TraitValues = allTraits.Where(x => x.JobCategories.Any(y => y.ItemId == z.ItemId)).Select(p => p.Title!.ToUpperInvariant()),
                    //Doesn't appear to be used in the internal workings out
                    NormalizedTotal = trait.TotalScore,
                    Total = allTraits.Count(x => x.JobCategories.Any(y => y.ItemId == z.ItemId)),
                    TotalQuestions = traitQuestions.Count(),
                }));
            }

            return results.OrderByDescending(t => t.Total).Take(10);
        }
    }
}
