﻿using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class CacheReloadService : ICacheReloadService
    {
        private readonly ILogger<CacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<ContentDysacModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;

        public CacheReloadService(
            ILogger<CacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<ContentDysacModel> eventMessageService,
            ICmsApiService cmsApiService,
            IContentCacheService contentCacheService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload cache started");

                await RemoveDuplicateCacheItems().ConfigureAwait(false);

                var summaryList = await GetSummaryListAsync().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload cache cancelled");

                    return;
                }

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryListAsync(summaryList, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("Reload cache cancelled");

                        return;
                    }

                    await DeleteStaleCacheEntriesAsync(summaryList, stoppingToken).ConfigureAwait(false);
                }

                logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache reload");
            }
        }

        public async Task RemoveDuplicateCacheItems()
        {
            logger.LogInformation("Removing duplicate cache items");

            var cachedContentPages = await eventMessageService.GetAllCachedItemsAsync().ConfigureAwait(false);
            var duplicates = cachedContentPages?.GroupBy(s => s.Url).SelectMany(grp => grp.Skip(1)).Select(t => t.Id).ToList();

            if (duplicates != null && duplicates.Any())
            {
                foreach (var id in duplicates)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (await eventMessageService.DeleteAsync(id).ConfigureAwait(false) == HttpStatusCode.OK)
                        {
                            break;
                        }
                    }
                }

                logger.LogInformation($"Removed {duplicates.Count} duplicate cache items");
            }
            else
            {
                logger.LogInformation("No duplicate items to be removed");
            }
        }

        public async Task<IList<ApiSummaryItemModel>?> GetSummaryListAsync()
        {
            logger.LogInformation("Get summary list");

            var summaryList = await cmsApiService.GetSummaryAsync<ApiSummaryItemModel>().ConfigureAwait(false);

            logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync(IList<ApiSummaryItemModel>? summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            contentCacheService.Clear();

            foreach (var item in summaryList.OrderByDescending(o => o.Published).ThenBy(o => o.Title))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItemAsync(item, stoppingToken).ConfigureAwait(false);
            }

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(ApiSummaryItemModel item, CancellationToken stoppingToken)
        {
            //_ = item ?? throw new ArgumentNullException(nameof(item));

            //try
            //{
            //    logger.LogInformation($"Get details for {item.Title} - {item.Url}");

            //    var apiDataModel = await cmsApiService.GetItemAsync<PagesApiDataModel, PagesApiContentItemModel>(item.Url!).ConfigureAwait(false);

            //    if (apiDataModel == null)
            //    {
            //        logger.LogWarning($"No details returned from {item.Title} - {item.Url}");

            //        return;
            //    }

            //    if (stoppingToken.IsCancellationRequested)
            //    {
            //        logger.LogWarning("Process item get and save cancelled");

            //        return;
            //    }

            //    var contentPageModel = mapper.Map<ContentDysacModel>(apiDataModel);

            //    if (!TryValidateModel(contentPageModel))
            //    {
            //        logger.LogError($"Validation failure for {item.Title} - {item.Url}");

            //        return;
            //    }

            //    logger.LogInformation($"Updating cache with {item.Title} - {item.Url}");

            //    var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            //    if (result == HttpStatusCode.NotFound)
            //    {
            //        logger.LogInformation($"Does not exist, creating cache with {item.Title} - {item.Url}");

            //        result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

            //        if (result == HttpStatusCode.Created)
            //        {
            //            logger.LogInformation($"Created cache with {item.Title} - {item.Url}");
            //        }
            //        else
            //        {
            //            logger.LogError($"Cache create error status {result} from {item.Title} - {item.Url}");
            //        }
            //    }
            //    else
            //    {
            //        logger.LogInformation($"Updated cache with {item.Title} - {item.Url}");
            //    }

            //    contentCacheService.AddOrReplace(contentPageModel.Id, contentPageModel.AllContentItemIds);
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, $"Error in get and save for {item.Title} - {item.Url}");
            //}

            return;
        }

        public async Task DeleteStaleCacheEntriesAsync(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = await eventMessageService.GetAllCachedItemsAsync().ConfigureAwait(false);

            if (cachedContentPages != null && cachedContentPages.Any())
            {
                var staleContentPages = cachedContentPages.Where(x => !summaryList.Any(z => z.Url == x.Url)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItemsAsync(staleContentPages, stoppingToken).ConfigureAwait(false);
                }
            }

            logger.LogInformation("Delete stale cache items completed");
        }

        public async Task DeleteStaleItemsAsync(List<ContentDysacModel> staleItems, CancellationToken stoppingToken)
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Delete stale cache items cancelled");

                    return;
                }

                logger.LogInformation($"Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await eventMessageService.DeleteAsync(staleContentPage.Id).ConfigureAwait(false);

                if (deletionResult == HttpStatusCode.OK)
                {
                    logger.LogInformation($"Deleted stale cache item {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
                else
                {
                    logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        public bool TryValidateModel(ContentPageModel contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
