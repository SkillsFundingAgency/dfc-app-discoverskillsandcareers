using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsService : IResultsService
    {
        private readonly ILogger<ResultsService> logger;
        private readonly ISessionService sessionService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;

        public ResultsService(
            ILogger<ResultsService> logger,
            IResultsApiService resultsApiService,
            IJpOverviewApiService jPOverviewAPIService,
            ISessionService sessionService,
            IAssessmentCalculationService assessmentCalculationService,
            IDocumentService<DysacAssessment> assessmentDocumentService)
        {
            this.logger = logger;
            //this.resultsApiService = resultsApiService;
            //this.jPOverviewAPIService = jPOverviewAPIService;
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

            switch (assessment.AssessmentState)
            {
                case Core.Enums.AssessmentState.Short:
                    return await ProcessShortAssessment(assessment).ConfigureAwait(false);
                case Core.Enums.AssessmentState.Filtered:
                    return await ProcessFilteredAssessment(assessment).ConfigureAwait(false);
                //return await ProcessShortAssessment(assessment).ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"State {assessment.AssessmentState} is not valid");
            }
        }

        private async Task<GetResultsResponse> ProcessShortAssessment(DysacAssessment assessment)
        {
            if (assessment.ShortQuestionResult != null)
            {
                //Return here, already calculated
            }

            var assessmentCalculationResponse = await assessmentCalculationService.ProcessAssessment(assessment).ConfigureAwait(false);
            await assessmentDocumentService.UpsertAsync(assessmentCalculationResponse).ConfigureAwait(false);

            return new GetResultsResponse { JobCategories = assessmentCalculationResponse.ShortQuestionResult.JobCategories, JobFamilyCount = assessmentCalculationResponse.ShortQuestionResult.JobCategories.Count(), JobProfiles = assessmentCalculationResponse.ShortQuestionResult.JobProfiles, Traits = assessmentCalculationResponse.ShortQuestionResult.TraitText, SessionId = assessment.AssessmentCode, AssessmentType = "short" };
        }

        private async Task<GetResultsResponse> ProcessFilteredAssessment(DysacAssessment assessment)
        {
            var answeredQuestions = assessment.FilteredAssessment.Questions.Where(z => z.Answer != null).Select(x => x.TraitCode).ToList();

            foreach (var jobCategory in assessment.FilteredAssessment.JobCategoryAssessments)
            {
                var remainingJobCategoryQuestionsCount = jobCategory.QuestionSkills.Count(x => !answeredQuestions.Contains(x.Key));
                assessment.ShortQuestionResult.JobCategories.FirstOrDefault(x => x.JobFamilyNameUrl == jobCategory.JobCategory).TotalQuestions = remainingJobCategoryQuestionsCount;
            }

            await assessmentDocumentService.UpsertAsync(assessment);


            return new GetResultsResponse { JobCategories = assessment.ShortQuestionResult.JobCategories, JobFamilyCount = assessment.ShortQuestionResult.JobCategories.Count(), JobProfiles = assessment.ShortQuestionResult.JobProfiles, Traits = assessment.ShortQuestionResult.TraitText, SessionId = assessment.AssessmentCode, AssessmentType = "filtered" };

        }

        public async Task<GetResultsResponse> GetResultsByCategory(string jobCategory)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var assessments = await assessmentDocumentService.GetAsync(x => x.AssessmentCode == sessionId).ConfigureAwait(false);

            if (assessments == null || !assessments.Any())
            {
                throw new InvalidOperationException($"Assessment {sessionId} is null");
            }

            var assessment = assessments.FirstOrDefault();

            foreach (var category in assessment.ShortQuestionResult.JobCategories)
            {
                //var selectedJobprofiles = resultsResponse.JobProfiles.Where(p => p.JobCategory == category.JobFamilyName).Select(p => p.UrlName);

                //if (selectedJobprofiles.Any())
                //{
                //    logger.LogInformation($"Getting Overview for {selectedJobprofiles.Count()} profiles for category {category.JobFamilyName}");
                //    category.JobProfilesOverviews = await jPOverviewAPIService.GetOverviewsForProfilesAsync(selectedJobprofiles).ConfigureAwait(false);
                //    logger.LogInformation($"Got Overview for {category.JobProfilesOverviews?.Count()} profiles for category {category.JobFamilyName}");
                //}
            }

            //resultsResponse.JobCategories = OrderResults(resultsResponse.JobCategories, jobCategory);
            //return resultsResponse;

            //await Task.Delay(0);
            return new GetResultsResponse() { JobCategories = assessment.ShortQuestionResult.JobCategories };
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
