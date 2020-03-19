using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IResultsService resultsService;
        private readonly IAssessmentService apiService;
        private readonly ExternalLinkOptions externalLinkOptions;

        public ResultsController(IMapper mapper, ISessionService sessionService, IResultsService resultsService, IAssessmentService apiService, ExternalLinkOptions externalLinkOptions)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.resultsService = resultsService;
            this.apiService = apiService;
            this.externalLinkOptions = externalLinkOptions;
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

            var resultIndexResponseViewModel = new ResultIndexResponseViewModel
            {
                Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse),
                AssessmentReference = assessmentResponse.ReferenceCode,
            };
            return View(resultIndexResponseViewModel);
        }

        public async Task<IActionResult> JobProfileOverviews(string category)
        {
            if (!await HasSessionId().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            category = "sports-and-leisure";

            var assessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await resultsService.GetResultsByCategory(category).ConfigureAwait(false);

            var resultsByCategoryModel = mapper.Map<ResultsByCategoryModel>(resultsResponse);
            resultsByCategoryModel.ExploreCareersUri = externalLinkOptions.ExploreCareers;
            resultsByCategoryModel.AssessmentReference = assessmentResponse.ReferenceCode;

            return View("ResultsByCategory", resultsByCategoryModel);
        }
    }
}