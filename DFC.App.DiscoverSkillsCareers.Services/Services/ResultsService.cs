using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsService : IResultsService
    {
        private readonly ILogger<ResultsService> logger;
        private readonly IResultsApiService resultsApiService;
        private readonly IJpOverviewApiService jPOverviewAPIService;
        private readonly ISessionService sessionService;

        public ResultsService(
            ILogger<ResultsService> logger,
            IResultsApiService resultsApiService,
            IJpOverviewApiService jPOverviewAPIService,
            ISessionService sessionService)
        {
            this.logger = logger;
            this.resultsApiService = resultsApiService;
            this.jPOverviewAPIService = jPOverviewAPIService;
            this.sessionService = sessionService;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            return await resultsApiService.GetResults(sessionId, AssessmentTypeName.ShortAssessment).ConfigureAwait(false);
        }

        public async Task<GetResultsResponse> GetResultsByCategory(string jobCategory)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var resultsResponse = await resultsApiService.GetResults(sessionId, jobCategory).ConfigureAwait(false);
            var selectedJobprofiles = resultsResponse.JobProfiles.Select(p => p.UrlName);

            if (selectedJobprofiles.Any())
            {
                logger.LogInformation($"Getting Overview for {selectedJobprofiles.Count()} profiles");
                resultsResponse.JobProfilesOverviews = await jPOverviewAPIService.GetOverviewsForProfilesAsync(selectedJobprofiles).ConfigureAwait(false);
                logger.LogInformation($"Got Overview for {resultsResponse.JobProfilesOverviews?.Count()} profiles");
            }

            return resultsResponse;
        }
    }
}
