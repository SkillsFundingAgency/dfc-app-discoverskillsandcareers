using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IJpOverviewApiService
    {
        IEnumerable<string> GetOverviewsForProfiles(IEnumerable<string> jobProfileNames);
    }
}
