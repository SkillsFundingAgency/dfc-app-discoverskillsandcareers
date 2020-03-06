using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IJpOverviewApiService
    {
       Task<IEnumerable<JobProfileOverView>> GetOverviewsForProfilesAsync(IEnumerable<string> jobProfileNames);
    }
}
