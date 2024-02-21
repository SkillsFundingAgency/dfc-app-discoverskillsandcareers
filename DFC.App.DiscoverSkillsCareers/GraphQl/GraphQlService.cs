using AutoMapper;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Logger.AppInsights.Contracts;
using Razor.Templating.Core;
using System.IO;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.GraphQl
{
    public class GraphQlService : IGraphQlService
    {
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly IMapper mapper;
        private readonly ILogService logService;

        public GraphQlService(ISharedContentRedisInterface sharedContentRedisInterface, IRazorTemplateEngine razorTemplateEngine, IMapper mapper, ILogService logService)
        {
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
            this.mapper = mapper;
            this.logService = logService;
        }

        public async Task<JobProfileViewModel> GetJobProfileAsync(string jobProfile)
        {
            var response = await sharedContentRedisInterface.GetDataAsync<JobProfileDysacResponse>($"JobProfileOverview/{jobProfile}")
                ?? new JobProfileDysacResponse();

            var mappedResponse = mapper.Map<JobProfileViewModel>(response);

            try
            {
                logService.LogInformation($"Attempting to build HTML for {jobProfile}");

                var html = await razorTemplateEngine.RenderAsync("~/Views/Results/JobProfileOverview.cshtml", mappedResponse).ConfigureAwait(false);
                mappedResponse.Html = html;
            }
            catch (IOException ex)
            {
                logService.LogError("Error: " + ex.GetType().Name + " - " + ex.Message);
            }

            return mappedResponse;
        }
    }
}
