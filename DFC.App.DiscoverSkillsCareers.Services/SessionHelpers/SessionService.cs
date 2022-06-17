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

        public SessionService(ISessionStateService<DfcUserSession> sessionStateService, IHttpContextAccessor accessor)
        {
            this.sessionStateService = sessionStateService;
            this.accessor = accessor;
        }

        public async Task<SessionStateModel<DfcUserSession>?> GetCurrentSession()
        {
            const string httpContextSessionKey = "DysacSession";

            if (accessor.HttpContext.Items.ContainsKey(httpContextSessionKey))
            {
                return (SessionStateModel<DfcUserSession>?)accessor.HttpContext.Items[httpContextSessionKey];
            }

            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();

            if (!compositeSessionId.HasValue)
            {
                return null;
            }

            var session = await sessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);
            accessor.HttpContext.Items.Add(httpContextSessionKey, session);

            return session;
        }

        public async Task CreateCookie(string sessionId)
        {
            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();

            var dfcUserSession = new SessionStateModel<DfcUserSession>
            {
                Id = compositeSessionId!.Value,
                State = new DfcUserSession
                {
                    Salt = "ncs",
                    SessionId = sessionId,
                    CreatedDate = DateTime.UtcNow,
                },
            };

            await sessionStateService.SaveAsync(dfcUserSession).ConfigureAwait(false);
        }

        public async Task<string> GetSessionId()
        {
            var session = await GetCurrentSession().ConfigureAwait(false);

            if (session?.State!.SessionId != null)
            {
                return session.State.SessionId;
            }

            throw new InvalidOperationException("SessionId is null or empty");
        }

        public async Task<bool> HasValidSession()
        {
            var session = await GetCurrentSession().ConfigureAwait(false);
            return session != null;
        }
    }
}
