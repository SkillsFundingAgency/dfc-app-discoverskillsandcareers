using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class JobCategoryHelper
    {
        public static async Task<List<DysacJobProfileCategoryContentModel>?> GetJobCategories(
            ISharedContentRedisInterface sharedContentRedisInterface,
            IMapper mapper,
            IConfiguration configuration)
        {
            string status = configuration?.GetSection("contentMode:contentMode").Get<string>();

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            double expiryInHours = 4;
            if (configuration != null)
            {
                string expiryAppString = configuration.GetSection("Cms:Expiry").Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }

            var result = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCategoriesResponseDysac>(Constants.DYSACJobProfileCategories, status, expiryInHours)
                   ?? new JobProfileCategoriesResponseDysac();

            var jobCategories = mapper.Map<List<DysacJobProfileCategoryContentModel>>(result.JobProfileCategories);
            return jobCategories.Where(x => x.Title != null).ToList();
        }
    }
}
