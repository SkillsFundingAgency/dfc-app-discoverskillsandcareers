using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsService : IResultsService
    {
        private readonly ISessionService sessionService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;

        public ResultsService(
            ISessionService sessionService,
            IAssessmentCalculationService assessmentCalculationService,
            IDocumentService<DysacAssessment> assessmentDocumentService)
        {
            this.sessionService = sessionService;
            this.assessmentCalculationService = assessmentCalculationService;
            this.assessmentDocumentService = assessmentDocumentService;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

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
            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assessment {sessionId} is null");
            }

            var assessment = assessments.FirstOrDefault();

            var answeredQuestions = assessment.FilteredAssessment!.Questions.Where(z => z.Answer != null).Select(x => x.TraitCode).ToList();

            foreach (var jobCategory in assessment.FilteredAssessment.JobCategoryAssessments)
            {
                var remainingJobCategoryQuestionsCount = jobCategory.QuestionSkills.Count(x => !answeredQuestions.Contains(x.Key));
                assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategory.JobCategory).TotalQuestions = remainingJobCategoryQuestionsCount;
            }

            await assessmentDocumentService.UpsertAsync(assessment).ConfigureAwait(false);

            var answeredPositiveQuestions = assessment.FilteredAssessment.Questions.Where(x => x.Answer != null && x.Answer!.Value == Core.Enums.Answer.Yes).Select(z => z.TraitCode).ToList();

            var selectedJobCategory = assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategoryName);

            var listOfJobProfiles = new List<JobProfileResult>();

            foreach (var jobProfile in selectedJobCategory.JobProfiles.Where(x => x.SkillCodes != null))
            {
                bool canAddJobProfile = true;

                foreach (var skill in jobProfile.SkillCodes)
                {
                    if (!answeredPositiveQuestions.Contains(skill))
                    {
                        canAddJobProfile = false;
                    }
                }

                if (canAddJobProfile)
                {
                    listOfJobProfiles.Add(jobProfile);
                }
            }

            assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategoryName).JobProfiles = listOfJobProfiles;

            // resultsResponse.JobCategories = OrderResults(resultsResponse.JobCategories, jobCategory);

            return new GetResultsResponse() { JobCategories = assessment.ShortQuestionResult.JobCategories };
        }


        private async Task<GetResultsResponse> ProcessAssessment(DysacAssessment assessment)
        {
            var assessmentCalculationResponse = await assessmentCalculationService.ProcessAssessment(assessment).ConfigureAwait(false);

            if (assessmentCalculationResponse == null)
            {
                throw new InvalidOperationException($"Assessment Caluclation Response is null for {assessment.AssessmentCode}");
            }

            await assessmentDocumentService.UpsertAsync(assessmentCalculationResponse).ConfigureAwait(false);

            return new GetResultsResponse { LastAssessmentCategory = assessment.FilteredAssessment?.JobCategoryAssessments.OrderByDescending(x => x.LastAnswer).FirstOrDefault()?.JobCategory!, JobCategories = assessmentCalculationResponse.ShortQuestionResult?.JobCategories!, JobFamilyCount = assessmentCalculationResponse.ShortQuestionResult?.JobCategories.Count(), JobProfiles = assessmentCalculationResponse.ShortQuestionResult?.JobProfiles, Traits = assessmentCalculationResponse.ShortQuestionResult?.TraitText!, SessionId = assessment.AssessmentCode!, AssessmentType = "short" };
        }

        //private IEnumerable<JobCategoryResult> OrderResults(IEnumerable<JobCategoryResult> categories, string selectedCategory)
        //{
        //    foreach (var c in categories)
        //    //{
        //    //    c.DisplayOrder = c.FilterAssessment?.SuggestedJobProfiles.Count();
        //    //    if (c.JobFamilyNameUrl == selectedCategory.ToLower()?.Replace(" ", "-"))
        //    //    {
        //    //        c.DisplayOrder = 9999;
        //    //    }
        //    //}

        //    return categories;
        //}
    }
}
