using AutoMapper;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Razor.Templating.Core;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.GraphQl
{
    public class GraphQlService : IGraphQlService
    {
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly IMapper mapper;

        public GraphQlService(ISharedContentRedisInterface sharedContentRedisInterface, IRazorTemplateEngine razorTemplateEngine, IMapper mapper)
        {
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
            this.mapper = mapper;
        }

        public async Task<JobProfileViewModel> GetJobProfileAsync(string jobProfile)
        {
            var response = await sharedContentRedisInterface.GetDataAsync<JobProfileOverviewResponse>($"JobProfileOverview/{jobProfile}")
                ?? new JobProfileOverviewResponse();

            var mappedResponse = mapper.Map<JobProfileViewModel>(response);

            mappedResponse.Html = await razorTemplateEngine.RenderAsync("~/Views/Results/JobProfileOverview.cshtml", mappedResponse);

            return mappedResponse;
        }
    }
}
