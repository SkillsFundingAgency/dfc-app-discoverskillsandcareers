using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Sessionstate;
using Dfc.Session.Models;
using Microsoft.AspNetCore.Http;
using System;
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

        public async Task<SessionStateModel<DfcUserSession>?> GetCurrentSession()
        {
            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                var sessionStateModel = await sessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);

                if (sessionStateModel != null)
                {
                    return sessionStateModel;
                }
            }

            return null;
        }

        public async Task CreateCookie(string sessionId)
        {
            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();

            var dfcUserSession = new SessionStateModel<DfcUserSession> { Id = compositeSessionId!.Value, State = new DfcUserSession() { Salt = "ncs", SessionId = sessionId, CreatedDate = DateTime.UtcNow } };

            await sessionStateService.SaveAsync(dfcUserSession).ConfigureAwait(false);
        }

        public async Task<string> GetSessionId()
        {
            var session = await GetCurrentSession().ConfigureAwait(false);

            if (session != null && session.State!.SessionId != null)
            {
                return session.State!.SessionId;
            }

            throw new InvalidOperationException("SessionId is null or empty");
        }

        public async Task<bool> HasValidSession()
        {
            var session = await GetCurrentSession().ConfigureAwait(false);

            if (session != null)
            {
                return true;
            }

            return false;
        }
    }
}
