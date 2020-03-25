using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISessionService
    {
        public Task<string> GetSessionId();

        public void CreateCookie(string sessionIdAndPartionKey);

        public Task<bool> HasValidSession();
    }
}
