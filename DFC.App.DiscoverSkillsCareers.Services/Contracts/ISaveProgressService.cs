using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public interface ISaveProgressService
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailRequest emailRequest);

        Task<string> ReloadAsync(string referenceCode);
    }
}
