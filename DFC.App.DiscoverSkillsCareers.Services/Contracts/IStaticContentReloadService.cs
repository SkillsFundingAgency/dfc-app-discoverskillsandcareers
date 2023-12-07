using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IStaticContentReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}