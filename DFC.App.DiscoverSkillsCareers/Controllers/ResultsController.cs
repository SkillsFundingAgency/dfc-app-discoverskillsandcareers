using AutoMapper;
using DFC.App.DiscoverSkillsCareers.GraphQl;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
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
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly Guid sharedContentItemGuid;
        private readonly IGraphQlService graphQlService;

        public ResultsController(
            ILogService logService,
            IMapper mapper,
            ISessionService sessionService,
            IResultsService resultsService,
            IAssessmentService assessmentService,
            IDocumentStore documentStore,
            IMemoryCache memoryCache,
            IDocumentService<StaticContentItemModel> staticContentDocumentService,
            IGraphQlService graphQlService,
            CmsApiClientOptions cmsApiClientOptions)
                : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.assessmentService = assessmentService;
            this.memoryCache = memoryCache;
            this.graphQlService = graphQlService;

            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.staticContentDocumentService = staticContentDocumentService;
            sharedContentItemGuid = new Guid(cmsApiClientOptions?.ContentIds ?? throw new ArgumentNullException(nameof(cmsApiClientOptions), "ContentIds cannot be null"));
        }

        [HttpGet]
        public async Task<IActionResult> Index(ResultsGetRequestViewModel requestViewModel)
        {
            logService.LogInformation($"ResultsController HttpGet Index");
            if (!await HasSessionId().ConfigureAwait(false))
            {
                logService.LogInformation($"ResultsController HttpGet Index RedirectToRoot");
                return RedirectToRoot();
            }

            var assessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
            var resultsResponse = await resultsService.GetResults(true).ConfigureAwait(false);

            if (resultsResponse.LastAssessmentCategory != null)
            {
                logService.LogInformation($"ResultsController HttpGet Index resultsResponse.LastAssessmentCategory {resultsResponse.LastAssessmentCategory}");
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
            logService.LogInformation($"ResultsController Roles id{id}");

            if (!await HasSessionId().ConfigureAwait(false))
            {
                logService.LogInformation($"ResultsController Roles id{id} RedirectToRoot");
                return RedirectToRoot();
            }

            var assessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                logService.LogInformation($"ResultsController Roles assessment/return");
                return RedirectTo("assessment/return");
            }

            logService.LogInformation("Assessment retrieved");

            var resultsResponse = await resultsService.GetResultsByCategory(id).ConfigureAwait(false);
            if (!resultsResponse.AllJobProfilesMatchWithAssessmentProfiles)
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

                var jobProfileList = await GetJobProfilesAsync(jobProfileTitles).ConfigureAwait(false);

                var category = resultsByCategoryModel.JobsInCategory!
                    .FirstOrDefault(job => job.CategoryUrl.Contains(jobCategory.JobFamilyNameUrl!));

                category?.JobProfiles?.AddRange(jobProfileList
                    .GroupBy(jobProfileOverview => jobProfileOverview.DisplayText)
                    .Select(jobProfileOverviewGroup => jobProfileOverviewGroup.First())
                    .Select(jobProfileOverview => new ResultJobProfileOverViewModel
                    {
                        Cname = jobProfileOverview.DisplayText,
                        OverViewHTML = jobProfileOverview.Html ?? $"<a href='/job-profiles{jobProfileOverview.UrlName}'>{jobProfileOverview.DisplayText}</a>",
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
            logService.LogInformation($"HeroBanner id {id}");
            if (!await HasSessionId().ConfigureAwait(false))
            {

                logService.LogInformation($"HeroBanner id {id} RedirectToRoot");
                return RedirectToRoot();
            }

            var resultsResponse = await resultsService.GetResults(false).ConfigureAwait(false);
            var resultsHeroBannerViewModel = mapper.Map<ResultsHeroBannerViewModel>(resultsResponse);
            resultsHeroBannerViewModel.SpeakToAnAdviser = await staticContentDocumentService
                .GetByIdAsync(sharedContentItemGuid, StaticContentItemModel.DefaultPartitionKey).ConfigureAwait(false);

            logService.LogInformation($"{nameof(HeroBanner)} generated the model and ready to pass to the view");
            return View("HeroResultsBanner", resultsHeroBannerViewModel);
        }

        [HttpGet]
        [Route("bodytop/results")]
        [Route("bodytop/results/roles/{category}")]
        public IActionResult BodyTop()
        {
            logService.LogInformation($"BodyTop BodyTopEmpty");
            return View("BodyTopEmpty");
        }

        private async Task<List<JobProfileViewModel>> GetJobProfilesAsync(IEnumerable<string> listOfJobProfileNames)
        {
            var jobProfileList = new List<JobProfileViewModel>();

            foreach (var jobProfile in listOfJobProfileNames)
            {
                jobProfileList.Add(await graphQlService.GetJobProfileAsync(jobProfile));
            }

            return jobProfileList;
        }

        private async Task<List<DysacJobProfileOverviewContentModel>> GetJobProfileOverviews()
        {
            logService.LogInformation($"GetJobProfileOverviews");
            if (memoryCache.TryGetValue(nameof(GetJobProfileOverviews), out var filteringQuestionsFromCache))
            {
                return (List<DysacJobProfileOverviewContentModel>)filteringQuestionsFromCache;
            }

            var jobProfileOverviews = await documentStore.GetAllContentAsync<DysacJobProfileOverviewContentModel>("JobProfileOverview", "GetJobProfileOverviews").ConfigureAwait(false);

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