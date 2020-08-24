using DFC.App.DiscoverSkillsCareers.Models.Common;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ICmsApiService
    {
        Task<ContentItemModel>? GetContentAsync();
    }
}
