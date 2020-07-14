using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IEventGridSubscriptionService
    {
        Task<HttpStatusCode> CreateAsync();

        Task<HttpStatusCode> DeleteAsync();
    }
}
