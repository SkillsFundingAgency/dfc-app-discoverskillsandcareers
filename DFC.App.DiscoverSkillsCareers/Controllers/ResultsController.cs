using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IResultsService resultsService;
        private readonly IAssessmentService apiService;
        private readonly ILogService logService;

        public ResultsController(ILogService logService, IMapper mapper, ISessionService sessionService, IResultsService resultsService, IAssessmentService apiService)
            : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await resultsService.GetResults().ConfigureAwait(false);

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
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await resultsService.GetResultsByCategory(id).ConfigureAwait(false);
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