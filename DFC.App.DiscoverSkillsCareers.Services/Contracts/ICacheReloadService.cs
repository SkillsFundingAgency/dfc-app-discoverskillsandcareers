using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.Pages.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
