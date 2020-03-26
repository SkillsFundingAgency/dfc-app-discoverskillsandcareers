using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
            var results = await resultsApiService.GetResults(sessionId, AssessmentTypeName.ShortAssessment).ConfigureAwait(false);
            return AddInCategoryJobProfileCount(results);
        }

        public async Task<GetResultsResponse> GetResultsByCategory(string jobCategory)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
            var resultsResponse = await resultsApiService.GetResults(sessionId, jobCategory).ConfigureAwait(false);

            foreach (var category in resultsResponse.JobCategories)
            {
                var selectedJobprofiles = resultsResponse.JobProfiles.Where(p => p.JobCategory == category.JobFamilyName).Select(p => p.UrlName);

                if (selectedJobprofiles.Any())
                {
                    logger.LogInformation($"Getting Overview for {selectedJobprofiles.Count()} profiles for category {category.JobFamilyName}");
                    category.JobProfilesOverviews = await jPOverviewAPIService.GetOverviewsForProfilesAsync(selectedJobprofiles).ConfigureAwait(false);
                    logger.LogInformation($"Got Overview for {category.JobProfilesOverviews?.Count()} profiles for category {category.JobFamilyName}");
                }
            }

            resultsResponse.JobCategories = OrderResults(resultsResponse.JobCategories, jobCategory);
            return resultsResponse;
        }

        private IEnumerable<JobCategoryResult> OrderResults(IEnumerable<JobCategoryResult> categories, string selectedCategory)
        {
            foreach (var c in categories)
            {
                c.DisplayOrder = c.FilterAssessment?.SuggestedJobProfiles.Count();
                if (c.JobFamilyNameUrl == selectedCategory.ToLower()?.Replace(" ", "-"))
                {
                    c.DisplayOrder = 9999;
                }
            }

            return categories;
        }

        private GetResultsResponse AddInCategoryJobProfileCount(GetResultsResponse resultsResponse)
        {
            foreach (var c in resultsResponse.JobCategories)
            {
                c.NumberOfMatchedJobProfile = resultsResponse.JobProfiles.Count(p => p.JobCategory == c.JobFamilyUrl);
            }

            return resultsResponse;
        }
    }
}
