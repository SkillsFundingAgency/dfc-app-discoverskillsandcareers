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
                .SelectMany(jobProfile => jobProfile.Skills.Select(skill => skill.Title))
                .Distinct();

            var profilesBySkill = new Dictionary<string, IList<string>>();

            foreach (var skill in skills)
            {
                if (!profilesBySkill.TryGetValue(skill!, out var profiles))
                {
                    profiles = new List<string>();
                    profilesBySkill[skill!] = profiles;
                }

                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.Skills.Any(s => skill!.Equals(s.Title, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Add(jobProfile.Title!);
                    }
                }
            }

            var commonSkills = profilesBySkill
                .Where(skillGroup => skillGroup.Value.Count / (double)jobProfiles.Count >= percentage)
                .Select(skillGroup => skillGroup.Key)
                .ToList();

            return new HashSet<string>(commonSkills);
        }

        public static IEnumerable<SkillAttribute> GetSkillAttributes(
            this List<JobProfileContentItemModel> profiles,
            HashSet<string> prominentSkills,
            double maxProfileDistributionPercentage)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            var profilesBySkill =
                profiles
                    .SelectMany(jobProfileContentItemModel =>
                        jobProfileContentItemModel.SkillsToCompare(prominentSkills)
                            .Select(skillContentItemModel => new { Profile = jobProfileContentItemModel, Skill = skillContentItemModel }))
                    .GroupBy(s => s.Skill.Title).ToArray();

            var profileSkillAttributes = profilesBySkill.Select(skillGroup =>
            {
                var onetRank = skillGroup.Average(profileAndSkill => profileAndSkill.Skill.ONetRank ?? 0);
                var ncsRank = skillGroup.Average(profileAndSkill => 20 - (profileAndSkill.Skill.Ordinal ?? 0));
                var profileCount = (double)skillGroup.Count();
                var profilePercentage = Convert.ToDecimal(
                    (1.0 - ((profiles.Count - profileCount) / profiles.Count)) * 100.0);

                return new SkillAttribute
                {
                    ONetAttribute = skillGroup.Key,
                    CompositeRank = Convert.ToDouble(onetRank + (profilePercentage / 20.0m)),
                    ONetRank = Convert.ToDouble(onetRank),
                    NcsRank = ncsRank,
                    TotalProfilesWithSkill = (int)profileCount,
                    PercentageProfileWithSkill = Convert.ToDouble(profilePercentage),
                    ProfilesWithSkill = new HashSet<string>(
                        skillGroup
                        .Select(profileAndSkill => profileAndSkill.Profile.Title!), StringComparer.InvariantCultureIgnoreCase),
                };
            });

            var minProfileDistributionPercentage = 100 - maxProfileDistributionPercentage;

            // NOTE - recreating a bug in the old DYSAC that means stuff doesn't get filtered out when it should (percentage
            // is between 0-1 but should be 0-100). So instead of 25 and 75, update these values to 0.25 and 0.75
            minProfileDistributionPercentage /= 100;
            maxProfileDistributionPercentage /= 100;

            // NOTE - This means any profile that has more then 5 skills won't be matched first time - and some questions
            // will never be asked - consider raising the number higher
            var maxProfilesToTake = Math.Min(5, (int)Math.Log(profiles.Count, 2) - 2);

            return profileSkillAttributes
                .OrderByDescending(skillAttribute => skillAttribute.PercentageProfileWithSkill)
                .SkipWhile(skillAttribute => skillAttribute.PercentageProfileWithSkill < maxProfileDistributionPercentage)
                .TakeWhile(skillAttribute => skillAttribute.PercentageProfileWithSkill > minProfileDistributionPercentage)
                .Take(maxProfilesToTake)
                .OrderByDescending(skillAttribute => skillAttribute.CompositeRank)
                .ToArray();
        }
    }
}
