using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class JobCategorySkillMappingHelper
    {
        public static HashSet<string> CalculateCommonSkillsByPercentage(
            IList<JobProfileContentItemModel> jobProfiles,
            double percentage = 0.75)
        {
            if (jobProfiles == null)
            {
                throw new ArgumentNullException(nameof(jobProfiles));
            }

            var skills = jobProfiles
                .SelectMany(jobProfile => jobProfile.Skills.Select(s => s.Title))
                .Distinct();

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
                    if (jobProfile.Skills.Any(s => skill.Equals(s.Title, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Add(jobProfile.Title!);
                    }
                }
            }

            var commonSkills = result
                .Where(k => (k.Value.Count / (double)jobProfiles.Count) >= percentage)
                .Select(r => r.Key)
                .ToList();

            return new HashSet<string>(commonSkills);
        }

        public static IEnumerable<SkillAttribute> GetSkillAttributes(
            this IEnumerable<JobProfileContentItemModel> profiles,
            HashSet<string> prominentSkills,
            double maxProfileDistributionPercentage)
        {
            var totalProfileCount = profiles.Count();

            var profilesBySkill =
                profiles
                    .SelectMany(y =>
                        y.SkillsToCompare(prominentSkills).Select(s => new { Profile = y, Skill = s }))
                    .GroupBy(s => s.Skill.Title).ToArray();

            var profileSkillAttributes = profilesBySkill.Select(s =>
            {
                var onetRank = s.Average(r => r.Skill.ONetRank ?? 0);
                var ncsRank = s.Average(r => 20 - (r.Skill.Ordinal ?? 0));
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

            var minProfileDistributionPercentage = 100 - maxProfileDistributionPercentage;

            // NOTE - recreating a bug in the old DYSAC that means stuff doesn't get filtered out when it should (percentage
            // is between 0-1 but should be 0-100). So instead of 25 and 75, update these values to 0.25 and 0.75
            minProfileDistributionPercentage /= 100;
            maxProfileDistributionPercentage /= 100;

            // NOTE - This means any profile that has more then 5 skills won't be matched first time - and some questions
            // will never be asked - consider raising the number higher
            var maxProfilesToTake = Math.Min(5, (int)Math.Log(totalProfileCount, 2) - 2);

            return profileSkillAttributes
                .OrderByDescending(a => a.PercentageProfileWithSkill)
                .SkipWhile(a => a.PercentageProfileWithSkill < maxProfileDistributionPercentage)
                .TakeWhile(a => a.PercentageProfileWithSkill > minProfileDistributionPercentage)
                .Take(maxProfilesToTake)
                .OrderByDescending(att => att.CompositeRank)
                .ToArray();
        }
    }
}
