using DFC.App.DiscoverSkillsCareers.ViewModels;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.GraphQl
{
    public interface IGraphQlService
    {
        Task<JobProfileViewModel> GetJobProfileAsync(string jobProfile);

        string OverviewHTMLBuilder(JobProfileViewModel response);

        string ConvertCurrencyToGBP(string amountStr);
    }
}
