using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Dfc.Session;
using Dfc.Session.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.SessionHelpers
{
    public class Session : ISession
    {
        private readonly ISessionClient sessionClient;

        public Session(ISessionClient sessionClient)
        {
            this.sessionClient = sessionClient;
        }

        public void CreateCookie(string sessionIdAndPartionKey)
        {
            var sessionIdAndPartitionKeyDetails = GetSessionAndPartitionKey(sessionIdAndPartionKey);
            var dfcUserSession = new DfcUserSession() { Salt = "ncs", PartitionKey = sessionIdAndPartitionKeyDetails.Item1, SessionId = sessionIdAndPartitionKeyDetails.Item2 };
            sessionClient.CreateCookie(dfcUserSession, false);
        }

        public async Task<string> GetSessionId()
        {
            var result = await sessionClient.TryFindSessionCode().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException("SessionId is null or empty");
            }

            return result;
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
