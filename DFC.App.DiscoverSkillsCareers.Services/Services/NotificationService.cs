using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Notify.Interfaces;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationClient notificationClient;
        private readonly NotifyOptions notifyOptions;

        public NotificationService(INotificationClient notificationClient, NotifyOptions notifyOptions)
        {
            this.notificationClient = notificationClient;
            this.notifyOptions = notifyOptions;
        }

        public SendEmailResponse SendEmail(string domain, string emailAddress, string sessionIdUnformatted, string sessionIdFormatted)
        {
            var personalisation = GetPersonalisation(domain, sessionIdUnformatted, sessionIdFormatted);
            notificationClient.SendEmail(emailAddress, notifyOptions.EmailTemplateId, personalisation);

            return new SendEmailResponse { IsSuccess = true };
        }

        public SendSmsResponse SendSms(string domain, string mobileNumber, string sessionIdUnformatted, string sessionIdFormatted)
        {
            var personalisation = GetPersonalisation(domain, sessionIdUnformatted, sessionIdFormatted);
            notificationClient.SendSms(mobileNumber, notifyOptions.SmsTemplateId, personalisation);

            return new SendSmsResponse { IsSuccess = true };
        }

        private Dictionary<string, dynamic> GetPersonalisation(
            string domain,
            string sessionIdUnformatted,
            string sessionIdFormatted) => new Dictionary<string, dynamic>
            {
                { "session_id", sessionIdFormatted },
                { "assessment_date",DateTime.Now.ToString("dd MM yyyy") },
                { "reload_url",  $"{domain}/reload?sessionId={sessionIdUnformatted}" },
            };

    }
}
