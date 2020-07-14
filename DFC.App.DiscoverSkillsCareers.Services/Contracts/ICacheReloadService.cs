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

        //Task<IList<PagesSummaryItemModel>?> GetSummaryListAsync();

        //Task ProcessContentAsync(IList<PagesSummaryItemModel> summaryList, CancellationToken stoppingToken);

        //Task GetAndSaveItemAsync(PagesSummaryItemModel item, CancellationToken stoppingToken);

        //Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken);

        //Task DeleteStaleCacheEntriesAsync(IList<PagesSummaryItemModel> summaryList, CancellationToken stoppingToken);
    }
}
