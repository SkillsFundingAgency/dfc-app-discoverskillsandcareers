using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISession
    {
        public Task<string> GetSessionId();

        public void CreateCookie(string sessionIdAndPartionKey);
    }
}
