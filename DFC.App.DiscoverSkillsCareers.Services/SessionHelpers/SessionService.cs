using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Sessionstate;
using Dfc.Session.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.SessionHelpers
{
    public class SessionService : ISessionService
    {
        private readonly ISessionStateService<DfcUserSession> sessionStateService;
        private readonly IHttpContextAccessor accessor;

        public SessionService(ISessionStateService<DfcUserSession> sessionServiceClient, IHttpContextAccessor accessor)
        {
            this.sessionStateService = sessionServiceClient;
            this.accessor = accessor;
        }

        public async Task CreateCookie(string sessionIdAndPartionKey)
        {
            var sessionIdAndPartitionKeyDetails = GetSessionAndPartitionKey(sessionIdAndPartionKey);
            var dfcUserSession = new SessionStateModel<DfcUserSession> { State = new DfcUserSession() { Salt = "ncs", PartitionKey = sessionIdAndPartitionKeyDetails.Item1, SessionId = sessionIdAndPartitionKeyDetails.Item2 } };

            await sessionStateService.SaveAsync(dfcUserSession).ConfigureAwait(false);
        }

        public async Task<string> GetSessionId()
        {
            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                var sessionStateModel = await sessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);

                if (sessionStateModel != null)
                {
                    return sessionStateModel.State!.SessionId;
                }
            }

            throw new InvalidOperationException("SessionId is null or empty");
        }

        public async Task<bool> HasValidSession()
        {
            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                var sessionStateModel = await sessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);

                if (sessionStateModel != null)
                {
                    return true;
                }
            }

            return false;
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
