using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    interface IResultsService
    {
        Task<GetResultsResponse> GetResults(string sessionId);

        Task<GetResultsResponse> GetResults(string sessionId, string jobCategory);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

    }
}
