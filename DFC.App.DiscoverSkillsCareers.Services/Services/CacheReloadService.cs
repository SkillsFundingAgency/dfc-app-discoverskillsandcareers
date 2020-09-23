﻿using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
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
        private readonly List<ApiSummaryItemModel> loadedItems;
        private readonly ILogger<CacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;

        public CacheReloadService(
            ILogger<CacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService eventMessageService,
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

                await ReloadContentType<ApiQuestionSet, DysacQuestionSetContentModel>("personalityquestionset", stoppingToken).ConfigureAwait(false);
                await ReloadContentType<ApiTrait, DysacTrait>("personalitytrait", stoppingToken).ConfigureAwait(false);
                await ReloadContentType<ApiSkill, DysacSkill>("personalityskill", stoppingToken).ConfigureAwait(false);

                logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache reload");
            }
        }

        public async Task<IList<ApiSummaryItemModel>?> GetSummaryListAsync(string contentType)
        {
            logger.LogInformation("Get summary list");

            var summaryList = await cmsApiService.GetSummaryAsync<ApiSummaryItemModel>(contentType).ConfigureAwait(false);

            logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync<TModel, TDestModel>(string contentType, IList<ApiSummaryItemModel>? summaryList, CancellationToken stoppingToken)
            where TModel : class, IBaseContentItemModel<ApiGenericChild>
            where TDestModel : class, IDysacContentModel
        {
            logger.LogInformation("Process summary list started");

            foreach (var item in summaryList.OrderByDescending(o => o.Published).ThenBy(o => o.Title))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItemAsync<TModel, TDestModel>(contentType, item, stoppingToken).ConfigureAwait(false);
            }

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync<TModel, TDestModel>(string contentType, ApiSummaryItemModel item, CancellationToken stoppingToken)
            where TModel : class, IBaseContentItemModel<ApiGenericChild>
            where TDestModel : class, IDysacContentModel
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            try
            {
                logger.LogInformation($"Get details for {item.Title} - {item.Url}");

                var apiDataModel = await cmsApiService.GetItemAsync<TModel, ApiGenericChild>(item.Url!).ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    logger.LogWarning($"No details returned from {item.Title} - {item.Url}");

                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process item get and save cancelled");

                    return;
                }

                var destinationModel = mapper.Map<TDestModel>(apiDataModel);
                destinationModel.LastCached = DateTime.UtcNow;

                if (!TryValidateModel(destinationModel))
                {
                    logger.LogError($"Validation failure for {item.Title} - {item.Url}");

                    return;
                }

                logger.LogInformation($"Updating cache with {item.Title} - {item.Url}");

                var result = await eventMessageService.UpdateAsync(destinationModel).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    logger.LogInformation($"Does not exist, creating cache with {item.Title} - {item.Url}");

                    result = await eventMessageService.CreateAsync<TDestModel>(destinationModel).ConfigureAwait(false);

                    if (result == HttpStatusCode.Created)
                    {
                        logger.LogInformation($"Created cache with {item.Title} - {item.Url}");
                    }
                    else
                    {
                        logger.LogError($"Cache create error status {result} from {item.Title} - {item.Url}");
                    }
                }
                else
                {
                    logger.LogInformation($"Updated cache with {item.Title} - {item.Url}");
                }

                if (destinationModel.AllContentItemIds != null)
                {
                    contentCacheService.AddOrReplace(destinationModel.Id, destinationModel.AllContentItemIds, contentType);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in get and save for {item.Title} - {item.Url}");
            }
        }

        public async Task DeleteStaleCacheEntriesAsync<TDestModel>(IList<ApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
            where TDestModel : class, IDysacContentModel
        {
            logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = await eventMessageService.GetAllCachedItemsAsync<TDestModel>().ConfigureAwait(false);

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

        public async Task DeleteStaleItemsAsync<TModel>(List<TModel> staleItems, CancellationToken stoppingToken)
            where TModel : class, IDysacContentModel
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Delete stale cache items cancelled");

                    return;
                }

                logger.LogInformation($"Deleting cache with {staleContentPage.Id}");

                var deletionResult = await eventMessageService.DeleteAsync<TModel>(staleContentPage.Id).ConfigureAwait(false);

                if (deletionResult == HttpStatusCode.OK)
                {
                    logger.LogInformation($"Deleted stale cache item {staleContentPage.Id}");
                }
                else
                {
                    logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.Id}");
                }
            }
        }

        public bool TryValidateModel<TDestModel>(TDestModel contentPageModel)
            where TDestModel : IDysacContentModel
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.Id} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }

        private async Task ReloadContentType<TModel, TDestModel>(string contentType, CancellationToken stoppingToken)
           where TModel : class, IBaseContentItemModel<ApiGenericChild>
           where TDestModel : class, IDysacContentModel
        {
            var summaryList = await GetSummaryListAsync(contentType).ConfigureAwait(false);
            await DeleteStaleCacheEntriesAsync<TDestModel>(summaryList!, stoppingToken).ConfigureAwait(false);

            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning($"Reload cache cancelled for {contentType}");

                return;
            }

            if (summaryList != null && summaryList.Any())
            {
                await ProcessSummaryListAsync<TModel, TDestModel>(contentType, summaryList, stoppingToken).ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning($"Reload cache cancelled for {contentType}");
                }
            }
        }
    }
}
