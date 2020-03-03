using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IJpOverviewAPIService
    {
        IEnumerable<string> GetOverviewsForProfiles(IEnumerable<string> jobProfileNames);
    }
}
