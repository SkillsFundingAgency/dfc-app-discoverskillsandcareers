using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task<IList<ApiSummaryItemModel>?> GetSummaryListAsync();

        Task ProcessSummaryListAsync(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken);

        Task GetAndSaveItemAsync(ApiSummaryItemModel item, CancellationToken stoppingToken);

        Task DeleteStaleItemsAsync(List<DysacContentModel> staleItems, CancellationToken stoppingToken);

        Task DeleteStaleCacheEntriesAsync(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken);
    }
}
