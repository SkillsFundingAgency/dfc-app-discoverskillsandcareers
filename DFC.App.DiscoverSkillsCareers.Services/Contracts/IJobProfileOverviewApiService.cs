using DFC.App.DiscoverSkillsCareers.Models.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IJobProfileOverviewApiService
    {
        Task<List<ApiJobProfileOverview>> GetOverviews(List<CanonicalNameWithTitle> jobProfileCanonicalNames);

        Task<ApiJobProfileOverview> GetOverview(Uri url);
    }
}
