using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.SessionHelpers
{
    public class SessionService : ISessionService
    {
        public SessionService(ISessionStateService<SessionDataModel> sessionStateService)
        {
            SessionStateService = sessionStateService;
        }

        public void CreateCookie(string sessionIdAndPartionKey)
        {
            var sessionIdAndPartitionKeyDetails = GetSessionAndPartitionKey(sessionIdAndPartionKey);
            var dfcUserSession = new DfcUserSession()
                                {Salt = "ncs",
                                    PartitionKey = sessionIdAndPartitionKeyDetails.Item1,
                                    SessionId = sessionIdAndPartitionKeyDetails.Item2 };
            
            sessionServiceClient.CreateCookie(dfcUserSession, false);
            
            
        }

        protected ISessionStateService<SessionDataModel> SessionStateService { get; private set; }

        public async Task<string> GetSessionId()
        {
            var result = await sessionServiceClient.TryFindSessionCode().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException("SessionId is null or empty");
            }

            return result;
        }

        public async Task<bool> HasValidSession()
        {
            var result = await sessionServiceClient.TryFindSessionCode().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(result))
            {
                return false;
            }

            return true;
        }

        private static Tuple<string, string> GetSessionAndPartitionKey(string value)
        {
            var result = new Tuple<string, string>(string.Empty, string.Empty);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var segments = value.Split("-", StringSplitOptions.RemoveEmptyEntries);
                result = new Tuple<string, string>(segments.ElementAtOrDefault(0), segments.ElementAtOrDefault(1));
            }

            return result;
        }
    }
}
