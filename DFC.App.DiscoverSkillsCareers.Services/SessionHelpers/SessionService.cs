﻿using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Sessionstate;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.Session.Models;

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

        private async Task<SessionStateModel<DfcUserSession>?> GetCurrentSession()
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

        public async Task CreateCookie(string sessionIdAndPartionKey)
        {
            var sessionIdAndPartitionKeyDetails = GetSessionAndPartitionKey(sessionIdAndPartionKey);
            var dfcUserSession = new SessionStateModel<DfcUserSession> { State = new DfcUserSession() { Salt = "ncs", PartitionKey = sessionIdAndPartitionKeyDetails.Item1, SessionId = sessionIdAndPartitionKeyDetails.Item2 } };

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
