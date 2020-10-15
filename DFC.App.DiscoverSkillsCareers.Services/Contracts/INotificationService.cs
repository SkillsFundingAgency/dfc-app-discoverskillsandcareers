using DFC.App.DiscoverSkillsCareers.Models.Assessment;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface INotificationService
    {
        SendEmailResponse SendEmail(string domain, string emailAddress, string sessionId);

        SendSmsResponse SendSms(string domain, string mobileNumber, string sessionId);
    }
}
