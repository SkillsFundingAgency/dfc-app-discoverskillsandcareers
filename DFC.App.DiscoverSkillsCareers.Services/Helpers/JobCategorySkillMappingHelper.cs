using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class JobCategorySkillMappingHelper
    {
        public static HashSet<string> CalculateCommonSkillsToRemoveByPercentage(IList<JobProfileContentItemModel> jobProfiles, double percentage = 0.75)
        {
            if (jobProfiles == null)
            {
                throw new ArgumentNullException(nameof(jobProfiles));
            }

            var skills = jobProfiles.SelectMany(p => p.Skills.Select(s => s.Title!)).Distinct();
            var result = new Dictionary<string, IList<string>>();

            foreach (var skill in skills)
            {
                if (!result.TryGetValue(skill, out var profiles))
                {
                    profiles = new List<string>();
                    result[skill] = profiles;
                }

                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.Skills.Any(s =>
                        skill.Equals(s.Title, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Add(jobProfile.Title!);
                    }
                }
            }

            return new HashSet<string>(result.Where(k => (double)k.Value.Count / (double)jobProfiles.Count >= percentage).Select(r => r.Key));
        }

        public static IEnumerable<SkillAttribute> GetSkillAttributes(
            this IEnumerable<JobProfileContentItemModel> profiles,
            HashSet<string> prominentSkills,
            double maxProfileDistributionPercentage)
        {
            var totalProfileCount = profiles.Count();

            var profilesBySkill =
                profiles
                    .SelectMany(y => y.SkillsToCompare(prominentSkills).Select(s => new { Profile = y, Skill = s }))
                    .GroupBy(s => s.Skill.Title).ToArray();

            var profileSkillAttributes = profilesBySkill.Select(s =>
            {
                var onetRank = s.Average(r => r.Skill.ONetRank!.Value);
                var ncsRank = s.Average(r => 20 - r.Skill.Ordinal);
                var profileCount = (double)s.Count();
                var profilePercentage = Convert.ToDecimal(
                    (1.0 - ((totalProfileCount - profileCount) / totalProfileCount)) * 100.0);

                return new SkillAttribute
                {
                    ONetAttribute = s.Key,
                    CompositeRank = Convert.ToDouble(onetRank + (profilePercentage / 20.0m)),
                    ONetRank = Convert.ToDouble(onetRank),
                    NcsRank = ncsRank,
                    TotalProfilesWithSkill = (int)profileCount,
                    PercentageProfileWithSkill = Convert.ToDouble(profilePercentage),
                    ProfilesWithSkill = new HashSet<string>(s.Select(p => p.Profile.Title!), StringComparer.InvariantCultureIgnoreCase),
                };
            });

            return profileSkillAttributes.OrderByDescending(a => a.PercentageProfileWithSkill)
                    .SkipWhile(a => a.PercentageProfileWithSkill < maxProfileDistributionPercentage)
                    .TakeWhile(a => a.PercentageProfileWithSkill > (1 - maxProfileDistributionPercentage))
                    .Take(5)
                    .OrderByDescending(a => a.CompositeRank)
                    .ToArray();
        }
    }
}
