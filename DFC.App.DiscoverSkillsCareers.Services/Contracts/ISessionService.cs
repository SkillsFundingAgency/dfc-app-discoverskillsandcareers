using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISessionService
    {
        public Task<string> GetSessionId();

        public Task CreateCookie(string sessionIdAndPartionKey);

        public Task<bool> HasValidSession();

        //Task<SessionStateModel<UserSession>?> GetCurrentSession();

        //Task SaveSession(UserSession session);
    }
}
