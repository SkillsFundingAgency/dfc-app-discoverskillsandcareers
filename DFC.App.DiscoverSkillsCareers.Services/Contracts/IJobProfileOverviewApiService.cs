using DFC.App.DiscoverSkillsCareers.Models.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IJobProfileOverviewApiService
    {
        Task<List<ApiJobProfileOverview>> GetOverviews(List<string> jobProfileCanonicalNames);

        Task<ApiJobProfileOverview> GetOverview(Uri url);
    }
}
