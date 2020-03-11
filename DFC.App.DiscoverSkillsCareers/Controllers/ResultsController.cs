using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Dfc.Session;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IApiService apiService;

        public ResultsController(IMapper mapper, ISessionClient sessionClient, IApiService apiService)
            : base(sessionClient)
        {
            this.mapper = mapper;
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

            var resultsResponse = await apiService.GetResults().ConfigureAwait(false);

            var resultIndexResponseViewModel = new ResultIndexResponseViewModel
            {
                Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse),
                AssessmentReference = assessmentResponse.ReferenceCode,
            };
            return View(resultIndexResponseViewModel);
        }
    }
}