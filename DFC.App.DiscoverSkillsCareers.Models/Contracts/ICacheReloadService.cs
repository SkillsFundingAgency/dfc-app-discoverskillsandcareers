﻿using DFC.App.DiscoverSkillsCareers.Models.API;
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

        Task ProcessSummaryListAsync<TModel, TDestModel>(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
             where TModel : class, IBaseContentItemModel<ApiGenericChild>
             where TDestModel : class, IDysacContentModel;

        Task GetAndSaveItemAsync<TModel, TDestModel>(ApiSummaryItemModel item, CancellationToken stoppingToken)
              where TModel : class, IBaseContentItemModel<ApiGenericChild>
              where TDestModel : class, IDysacContentModel;

        Task DeleteStaleItemsAsync<TModel>(List<TModel> staleItems, CancellationToken stoppingToken)
            where TModel : class, IDysacContentModel;

        Task DeleteStaleCacheEntriesAsync<TDestModel>(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
            where TDestModel : class, IDysacContentModel;
    }
}