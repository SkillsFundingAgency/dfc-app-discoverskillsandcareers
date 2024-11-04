using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class CommonService : ICommonService
    {
        private const string ExpiryAppSettings = "Cms:Expiry";
        private const string HttpContextAssessmentKey = "DysacAssessment";

        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;
        private readonly IDocumentStore documentStore;
        private readonly IMapper mapper;
        private readonly INotificationService notificationService;
        private readonly IHttpContextAccessor accessor;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;
        private string status;
        private double expiryInHours = 4;

        public CommonService(
            ISessionIdToCodeConverter sessionIdToCodeConverter,
            ISessionService sessionService,
            IDocumentStore documentStore,
            IMapper mapper,
            INotificationService notificationService,
            IHttpContextAccessor accessor,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IConfiguration configuration)
        {
            this.sessionIdToCodeConverter = sessionIdToCodeConverter;
            this.sessionService = sessionService;
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.accessor = accessor;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.configuration = configuration;

            status = configuration?.GetSection("contentMode:contentMode").Get<string>();

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            if (this.configuration != null)
            {
                string expiryAppString = this.configuration.GetSection(ExpiryAppSettings).Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }
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
    }
}
