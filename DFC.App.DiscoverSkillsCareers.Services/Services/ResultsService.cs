using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class ResultsService : IResultsService
    {
        private readonly ILogger<ResultsService> logger;
        private readonly IResultsApiService resultsApiService;
        private readonly ISession session;

        public ResultsService(
            ILogger<ResultsService> logger,
            IResultsApiService resultsApiService,
            ISession session)
        {
            this.logger = logger;
            this.resultsApiService = resultsApiService;
            this.session = session;
        }

        public async Task<GetResultsResponse> GetResults()
        {
            var sessionId = await session.GetSessionId().ConfigureAwait(false);
            return await resultsApiService.GetResults(sessionId).ConfigureAwait(false);
        }

    }
}
