using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ICommonService
    {
        Task<SendEmailResponse> SendEmail(string domain, string emailAddress);

        Task<SendSmsResponse> SendSms(string domain, string mobile);
    }
}
