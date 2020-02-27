using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    interface IResultsService
    {
        Task<GetResultsResponse> GetResults(string jobCategory = null);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);
    }
}
