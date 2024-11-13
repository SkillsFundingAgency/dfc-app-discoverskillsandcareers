using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class CommonService : ICommonService
    {
        private readonly ISessionService sessionService;
        private readonly INotificationService notificationService;

        public CommonService(
            ISessionService sessionService,
            INotificationService notificationService)
        {
            this.sessionService = sessionService;
            this.notificationService = notificationService;
        }

        public async Task<SendEmailResponse> SendEmail(string domain, string emailAddress)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService
                .SendEmail(domain, emailAddress, sessionId, SessionHelper.FormatSessionId(sessionId));
        }

        public async Task<SendSmsResponse> SendSms(string domain, string mobile)
        {
            var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);

            return notificationService
                .SendSms(domain, mobile, sessionId, SessionHelper.FormatSessionId(sessionId));
        }

        public async Task<bool> HasSessionStateId()
        {
            var session = await sessionService.GetCurrentSession().ConfigureAwait(false);
            return !(session?.State?.SessionId is null);
        }
    }
}
