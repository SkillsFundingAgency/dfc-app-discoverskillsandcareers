using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IApiService apiService;

        public ResultsController(IMapper mapper, ISessionService sessionService, IApiService apiService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var resultsResponse = await apiService.GetResults().ConfigureAwait(false);

            var response = new ResultIndexResponseViewModel();
            response.Results = mapper.Map<ResultsIndexResponseViewModel>(resultsResponse);
            return View(response);
        }

        public IActionResult Filter()
        {
            return View();
        }


        private async Task<GetAssessmentResponse> GetAssessment()
        {
            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            return getAssessmentResponse;
        }
    }
}