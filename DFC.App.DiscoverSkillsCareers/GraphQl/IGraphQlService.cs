using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.GraphQl
{
    public interface IGraphQlService
    {
        Task<JobProfileViewModel> GetJobProfileAsync(string jobProfile);

        string OverviewHTMLBuilder(JobProfileOverviewResponse response);
    }
}
