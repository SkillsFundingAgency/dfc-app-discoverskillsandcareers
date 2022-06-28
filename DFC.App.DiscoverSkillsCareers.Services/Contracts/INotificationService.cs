using DFC.App.DiscoverSkillsCareers.Models.Assessment;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface INotificationService
    {
        SendEmailResponse SendEmail(string domain, string emailAddress, string sessionIdUnformatted, string sessionIdFormatted);

        SendSmsResponse SendSms(string domain, string mobileNumber, string sessionIdUnformatted, string sessionIdFormatted);
    }
}
