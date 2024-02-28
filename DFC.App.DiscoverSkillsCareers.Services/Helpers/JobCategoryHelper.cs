using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class JobCategoryHelper
    {
        public static async Task<List<DysacJobProfileCategoryContentModel>?> GetJobCategories(
            ISharedContentRedisInterface sharedContentRedisInterface,
            IMapper mapper
            )
        {
            var result = await sharedContentRedisInterface.GetDataAsync<JobProfileCategoriesResponse>("DYSAC/JobProfileCategories")
                   ?? new JobProfileCategoriesResponse();

            var jobCategories = mapper.Map<List<DysacJobProfileCategoryContentModel>>(result.JobProfileCategories);
            return jobCategories.Where(x => x.Title != null).ToList();
        }

        public static async Task<List<DysacJobProfileCategoryContentModel>?> GetJobCategories(
            IMemoryCache memoryCache,
            IDocumentStore documentStore
            )
        {
            if (memoryCache.TryGetValue(nameof(GetJobCategories), out var filteringQuestionsFromCache))
            {
                return (List<DysacJobProfileCategoryContentModel>?)filteringQuestionsFromCache;
            }

            var jobCategories = await documentStore
                .GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory").ConfigureAwait(false);

            if (jobCategories == null)
            {
                return jobCategories;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
            memoryCache.Set(nameof(GetJobCategories), jobCategories, cacheEntryOptions);

            return jobCategories;
        }
    }
}
