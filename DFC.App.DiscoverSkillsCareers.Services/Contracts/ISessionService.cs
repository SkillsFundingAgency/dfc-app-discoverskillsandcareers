using DFC.Compui.Sessionstate;
using Dfc.Session.Models;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISessionService
    {
        public Task<string> GetSessionId();

        public Task CreateCookie(string sessionId);

        public Task<bool> HasValidSession();

        public Task<SessionStateModel<DfcUserSession>?> GetCurrentSession();
    }
}
