using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task<IList<ApiSummaryItemModel>?> GetSummaryListAsync(string contentType);

        Task ProcessSummaryListAsync<TModel, TDestModel>(string contentType, IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
             where TModel : class, IBaseContentItemModel<ApiGenericChild>
             where TDestModel : class, IDocumentModel, IDysacContentModel;

        Task GetAndSaveItemAsync<TModel, TDestModel>(string contentType, ApiSummaryItemModel item, CancellationToken stoppingToken)
              where TModel : class, IBaseContentItemModel<ApiGenericChild>
              where TDestModel : class, IDocumentModel, IDysacContentModel;

        Task DeleteStaleItemsAsync<TModel>(List<TModel> staleItems, CancellationToken stoppingToken)
            where TModel : class, IDocumentModel, IDysacContentModel;

        Task DeleteStaleCacheEntriesAsync<TDestModel>(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
            where TDestModel : class, IDocumentModel, IDysacContentModel;
    }
}
