using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;

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
    }
}
