using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController<ResultsController>
    {
        private readonly IMapper mapper;
        private readonly IResultsService resultsService;
        private readonly IAssessmentService apiService;
        private readonly ILogService logService;

        public ResultsController(ILogger<ResultsController> logger, IMapper mapper, ISessionStateService<SessionDataModel> sessionStateService, IResultsService resultsService, IAssessmentService apiService)
            : base(logger, sessionStateService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await apiService.GetAssessment(Request.CompositeSessionId()).ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await resultsService.GetResults(Request.CompositeSessionId()).ConfigureAwait(false);

            var lastFilterCategory = resultsResponse.JobCategories
                                                  .Where(x => x.FilterAssessment != null)
                                                  .OrderByDescending(x => x.FilterAssessment.CreatedDt)
                                                  .FirstOrDefault();
            if (lastFilterCategory != null)
            {
                return RedirectTo($"results/roles/{lastFilterCategory.JobFamilyNameUrl}");
            }

            var resultIndexResponseViewModel = new ResultIndexResponseViewModel
            {
                Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse),
                AssessmentReference = assessmentResponse.ReferenceCode,
            };
            return View(resultIndexResponseViewModel);
        }

        public async Task<IActionResult> Roles(string id)
        {
            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await apiService.GetAssessment(Request.CompositeSessionId()).ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await resultsService.GetResultsByCategory(id, Request.CompositeSessionId()).ConfigureAwait(false);
            var resultsByCategoryModel = mapper.Map<ResultsByCategoryModel>(resultsResponse);
            resultsByCategoryModel.AssessmentReference = assessmentResponse.ReferenceCode;

            this.logService.LogInformation($"{nameof(this.Roles)} generated the model and ready to pass to the view");

            return View("ResultsByCategory", resultsByCategoryModel);
        }

        [HttpGet]
        [Route("herobanner/results")]
        [Route("herobanner/results/roles/{id}")]
        public async Task<IActionResult> HeroBanner(string id)
        {
            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var resultsResponse = await resultsService.GetResults(Request.CompositeSessionId()).ConfigureAwait(false);

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