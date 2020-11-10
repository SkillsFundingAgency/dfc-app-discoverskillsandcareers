using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    [ExcludeFromCodeCoverage]
    public class SkillAttribute
    {
        public SkillAttribute()
        {
            ProfilesWithSkill = new HashSet<string>();
        }

        public string? ONetAttribute { get; set; }

        public double? CompositeRank { get; set; }

        public double? ONetRank { get; set; }

        public double? NcsRank { get; set; }

        public int? TotalProfilesWithSkill { get; set; }

        public double? PercentageProfileWithSkill { get; set; }

        public HashSet<string> ProfilesWithSkill { get; set; }
    }
}
