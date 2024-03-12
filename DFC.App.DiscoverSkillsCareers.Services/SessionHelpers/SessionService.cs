using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Sessionstate;
using DFC.Logger.AppInsights.Contracts;
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
        private readonly ILogService logService;

        public SessionService(ISessionStateService<DfcUserSession> sessionStateService, IHttpContextAccessor accessor, ILogService logService)
        {
            this.sessionStateService = sessionStateService;
            this.accessor = accessor;
            this.logService = logService;
        }

        public async Task<SessionStateModel<DfcUserSession>?> GetCurrentSession()
        {
            logService.LogInformation($"SessionService GetCurrentSession started");
            const string httpContextSessionKey = "DysacSession";

            if (accessor.HttpContext.Items.ContainsKey(httpContextSessionKey))
            {
                logService.LogInformation($"SessionService -> GetCurrentSession -> Contains Session Key: " + httpContextSessionKey);
                return (SessionStateModel<DfcUserSession>?)accessor.HttpContext.Items[httpContextSessionKey];
            }

            var compositeSessionId = accessor.HttpContext.Request.CompositeSessionId();

            if (!compositeSessionId.HasValue)
            {
                logService.LogError($"SessionService-> GetCurrentSession -> compositeSessionId has no value.");
                return null;
            }

            var session = await sessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);
            accessor.HttpContext.Items.Add(httpContextSessionKey, session);

            return session;
        }

        public async Task CreateDysacSession(string sessionId)
        {
            logService.LogInformation($"SessionService CreateDysacSession started");
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
            logService.LogInformation($"SessionService GetSessionId started");
            var session = await GetCurrentSession().ConfigureAwait(false);

            if (session?.State?.SessionId != null)
            {
                logService.LogInformation($"SessionService GetSessionId: " + session?.State?.SessionId);
                return session.State.SessionId;
            }

            logService.LogError($"SessionService GetSessionId -> SessionId is null");
            throw new InvalidOperationException("SessionId is null or empty");
        }

        public async Task<bool> HasValidSession()
        {
            return await GetCurrentSession().ConfigureAwait(false) != null;
        }
    }
}
