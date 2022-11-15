using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IResultsService resultsService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;
        private readonly IMemoryCache memoryCache;
        private readonly IDocumentStore documentStore;

        public ResultsController(
            ILogService logService,
            IMapper mapper,
            ISessionService sessionService,
            IResultsService resultsService,
            IAssessmentService assessmentService,
            IDocumentStore documentStore,
            IMemoryCache memoryCache)
                : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.assessmentService = assessmentService;
            this.memoryCache = memoryCache;

            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        [HttpGet]
        public async Task<IActionResult> Index(ResultsGetRequestViewModel requestViewModel)
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
            var resultsResponse = await resultsService.GetResults(true).ConfigureAwait(false);

            if (resultsResponse.LastAssessmentCategory != null)
            {
                return RedirectTo($"results/roles/{resultsResponse.LastAssessmentCategory}");
            }

            var resultIndexResponseViewModel = new ResultIndexResponseViewModel
            {
                Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse),
                AssessmentReference = assessmentResponse.ReferenceCode,
            };

            resultIndexResponseViewModel.Results.JobCategoriesNumberToShow = requestViewModel?.CountToShow ?? 3;

            logService.LogInformation("About to display results view");
            return View(resultIndexResponseViewModel);
        }

        public async Task<IActionResult> Roles(string id)
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            logService.LogInformation("Assessment retrieved");

            var resultsResponse = await resultsService.GetResultsByCategory(id).ConfigureAwait(false);
            if (resultsResponse == null)
            {
                logService.LogError("List of all job profiles doesn't contain profile referenced in the assessment. Regenerating the results.");
                return RedirectTo("results");
            }

            var resultsByCategoryModel = mapper.Map<ResultsByCategoryModel>(resultsResponse);

            logService.LogInformation("Got results by category");

            if (!string.IsNullOrEmpty(id))
            {
                var category = resultsByCategoryModel?.JobsInCategory?.FirstOrDefault(jobsInCategory => jobsInCategory.CategoryUrl == id);

                if (category == null)
                {
                    if (resultsByCategoryModel?.JobsInCategory != null)
                    {
                        var ids = string.Join(',', resultsByCategoryModel.JobsInCategory
                            .Select(jobsInCategory => jobsInCategory.CategoryUrl).ToArray());

                        logService.LogError(
                            $"Category is null - found {resultsByCategoryModel.JobsInCategory?.Count()} categories. Received {ids}. None which were {id}");
                    }
                }
                else
                {
                    category.ShowThisCategory = true;
                }
            }

            if (resultsByCategoryModel?.JobsInCategory != null)
            {
                var fullAssessment = await assessmentService.GetAssessment(assessmentResponse.SessionId!).ConfigureAwait(false);

                if (fullAssessment.FilteredAssessment != null)
                {
                    foreach (var category in resultsByCategoryModel.JobsInCategory)
                    {
                        if (fullAssessment.FilteredAssessment!.JobCategoryAssessments.Any(filteredJobCategory =>
                                filteredJobCategory.JobCategory == category.CategoryUrl
                                && filteredJobCategory.LastAnswer != default))
                        {
                            category.ShowThisCategory = true;
                        }
                    }
                }
            }

            logService.LogInformation($"Looping through categories - {resultsResponse.JobCategories?.Count()} available");

            if (resultsResponse.JobCategories == null)
            {
                throw new NoNullAllowedException(nameof(resultsResponse.JobCategories));
            }

            if (resultsByCategoryModel == null)
            {
                throw new NoNullAllowedException(nameof(resultsByCategoryModel));
            }

            foreach (var jobCategory in resultsResponse.JobCategories)
            {
                if (!jobCategory.JobProfiles.Any())
                {
                    logService.LogInformation($"No job profiles found for {jobCategory.JobFamilyName} - skipping");
                    continue;
                }

                var jobProfileTitles = jobCategory.JobProfiles
                    .GroupBy(jobProfile => jobProfile.Title)
                    .Select(jobProfileGroup => jobProfileGroup.First())
                    .Select(jobProfile => jobProfile?.Title?.ToLower());

                var jobProfileOverviews = (await GetJobProfileOverviews().ConfigureAwait(false))?
                    .Where(document => jobProfileTitles.Contains(document.Title!.ToLower()))
                    .ToList();

                if (jobProfileOverviews == null)
                {
                    logService.LogError(
                        $"Job Profile Overviews returned null for: {string.Join(",", jobProfileTitles)}");

                    continue;
                }

                var category = resultsByCategoryModel.JobsInCategory!
                    .FirstOrDefault(job => job.CategoryUrl.Contains(jobCategory.JobFamilyNameUrl!));

                category?.JobProfiles?.AddRange(jobProfileOverviews
                    .GroupBy(jobProfileOverview => jobProfileOverview.Title)
                    .Select(jobProfileOverviewGroup => jobProfileOverviewGroup.First())
                    .Select(jobProfileOverview => new ResultJobProfileOverViewModel
                    {
                        Cname = jobProfileOverview.Title?.Replace(" ", "-").Replace(",", string.Empty),
                        OverViewHTML = jobProfileOverview.Html ?? $"<a href='/job-profiles{jobProfileOverview.Url}'>{jobProfileOverview.Title}</a>",
                        ReturnedStatusCode = System.Net.HttpStatusCode.OK,
                    }));
            }

            logService.LogInformation("Finished loop");

            resultsByCategoryModel.AssessmentReference = assessmentResponse.ReferenceCode;
            resultsByCategoryModel.AssessmentType = "filter";

            logService.LogInformation($"{nameof(Roles)} generated the model and ready to pass to the view");

            return View("ResultsByCategory", resultsByCategoryModel);
        }

        [HttpGet]
        [Route("herobanner/results")]
        [Route("herobanner/results/roles/")]
        [Route("herobanner/results/roles/{id}")]
        public async Task<IActionResult> HeroBanner(string id)
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var resultsResponse = await resultsService.GetResults(false).ConfigureAwait(false);
            var resultsHeroBannerViewModel = mapper.Map<ResultsHeroBannerViewModel>(resultsResponse);

            logService.LogInformation($"{nameof(HeroBanner)} generated the model and ready to pass to the view");
            return View("HeroResultsBanner", resultsHeroBannerViewModel);
        }

        [HttpGet]
        [Route("bodytop/results")]
        [Route("bodytop/results/roles/{category}")]
        public IActionResult BodyTop()
        {
            return View("BodyTopEmpty");
        }

        private async Task<List<DysacJobProfileOverviewContentModel>> GetJobProfileOverviews()
        {
            if (memoryCache.TryGetValue(nameof(GetJobProfileOverviews), out var filteringQuestionsFromCache))
            {
                return (List<DysacJobProfileOverviewContentModel>)filteringQuestionsFromCache;
            }

            var jobProfileOverviews = await documentStore.GetAllContentAsync<DysacJobProfileOverviewContentModel>(
                "JobProfileOverview").ConfigureAwait(false);

            if (!jobProfileOverviews.Any())
            {
                return jobProfileOverviews;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
            memoryCache.Set(nameof(GetJobProfileOverviews), jobProfileOverviews, cacheEntryOptions);

            return jobProfileOverviews;
        }
    }
}