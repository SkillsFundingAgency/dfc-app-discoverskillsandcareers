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
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var assessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessmentResponse.IsComplete && !assessmentResponse.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var resultsResponse = await apiService.GetResults().ConfigureAwait(false);

            var response = new ResultIndexResponseViewModel();
            response.Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse);
            return View(response);
        }

        public IActionResult Filter()
        {
            return View();
        }
    }
}