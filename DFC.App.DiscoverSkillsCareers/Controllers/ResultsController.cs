using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IResultsService resultsService;
        private readonly IAssessmentService assessmentService;
        private readonly IDocumentService<DysacJobProfileOverviewContentModel> jobProfileOverviewDocumentService;
        private readonly ILogService logService;

        public ResultsController(ILogService logService, IMapper mapper, ISessionService sessionService, IResultsService resultsService, IAssessmentService apiService, IDocumentService<DysacJobProfileOverviewContentModel> jobProfileOverviewDocumentService)
            : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.assessmentService = apiService;
            this.jobProfileOverviewDocumentService = jobProfileOverviewDocumentService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
            if (assessmentResponse == null)
            {
                logService.LogInformation("Assesment is null");
                return RedirectTo("assessment/return");
            }

            logService.LogInformation("Assessment is not null");

            var resultsResponse = await resultsService.GetResults().ConfigureAwait(false);

            var lastFilterCategory = resultsResponse.LastAssessmentCategory;

            if (lastFilterCategory != null)
            {
                return RedirectTo($"results/roles/{lastFilterCategory}");
            }

            var resultIndexResponseViewModel = new ResultIndexResponseViewModel
            {
                Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse),
                AssessmentReference = assessmentResponse.ReferenceCode,
            };

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

            logService.LogInformation("Assesment retrieved");

            var resultsResponse = await resultsService.GetResultsByCategory(id).ConfigureAwait(false);
            var resultsByCategoryModel = mapper.Map<ResultsByCategoryModel>(resultsResponse);

            logService.LogInformation("Got results by category");

            if (!string.IsNullOrEmpty(id))
            {
                var category = resultsByCategoryModel?.JobsInCategory?.FirstOrDefault(x => x.CategoryUrl == id);

                if (category == null)
                {
                    if (resultsByCategoryModel?.JobsInCategory != null)
                    {
                        var ids = string.Join(',', resultsByCategoryModel?.JobsInCategory.Select(x => x.CategoryUrl).ToArray());
 
                        logService.LogError(
                            $"Category is null - found {resultsByCategoryModel?.JobsInCategory?.Count()} categories. Received {ids}. None which were {id}");
                    }
                }
                else
                {
                    category.ShowThisCategory = true;
                }
            }

            logService.LogInformation($"Looping through categories - {resultsResponse?.JobCategories?.Count()} available");

            if (resultsResponse?.JobCategories == null)
            {
                throw new Exception("Job categories is null");
            }

            if (resultsByCategoryModel == null)
            {
                throw new Exception("Results by category model is null");
            }

            foreach (var jobCategory in resultsResponse.JobCategories)
            {
                if (jobCategory?.JobProfiles.Any() != true)
                {
                    logService.LogInformation($"No job profiles found for {jobCategory.JobFamilyName} - skipping");
                    continue;
                }

                var jobProfileTitles = jobCategory.JobProfiles.GroupBy(y => y.Title).Select(
                    x => x.FirstOrDefault()).Select(z => z?.Title?.ToLowerInvariant());
                var jobProfileOverviews = await jobProfileOverviewDocumentService.GetAsync(
                        x => x.PartitionKey == "JobProfileOverview"
                             && jobProfileTitles.Contains(x.Title.ToLower()))
                    .ConfigureAwait(false);

                if (jobProfileOverviews == null)
                {
                    logService.LogError(
                        $"Job Profile Overviews returned null for: {string.Join(",", jobProfileTitles)}");

                    continue;
                }

                var category = resultsByCategoryModel.JobsInCategory.FirstOrDefault(x =>
                    x.CategoryUrl.Contains(jobCategory.JobFamilyNameUrl));

                category?.JobProfiles?.AddRange(jobProfileOverviews
                    .GroupBy(x => x.Title)
                    .Select(x => x.First())
                    .Select(x => new ResultJobProfileOverViewModel
                    {
                        Cname = x.Title.Replace(" ", "-"),
                        OverViewHTML = x.Html ?? $"<a href='/job-profiles{x.Url}'>{x.Title}</a>",
                        ReturnedStatusCode = System.Net.HttpStatusCode.OK
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
        [Route("herobanner/results/roles/{id}")]
        public async Task<IActionResult> HeroBanner(string id)
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var resultsResponse = await resultsService.GetResults().ConfigureAwait(false);

            var resultsHeroBannerViewModel = mapper.Map<ResultsHeroBannerViewModel>(resultsResponse);

            // if a category is passed in then we are on the roles page and do not want to display category text
            resultsHeroBannerViewModel.IsCategoryBanner = string.IsNullOrEmpty(id);

            this.logService.LogInformation($"{nameof(this.HeroBanner)} generated the model and ready to pass to the view");

            return View("HeroResultsBanner", resultsHeroBannerViewModel);
        }

        [HttpGet]
        [Route("bodytop/results")]
        [Route("bodytop/results/roles/{category}")]
        public IActionResult BodyTop()
        {
            return View("BodyTopEmpty");
        }
    }
}