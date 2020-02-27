using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public interface ISaveProgressService
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailRequest emailRequest);

        Task<string> ReloadAsync(string referenceCode);
    }
}