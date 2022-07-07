using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfileContentItemModel : IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        public Uri? Url { get; set; }

        public string? Title { get; set; }

        public DateTime? LastCached { get; set; }

        public List<DysacSkillContentItemModel> Skills { get; set; } = new List<DysacSkillContentItemModel>();

        public List<Guid>? AllContentItemIds => new List<Guid>();

        public int? Ordinal { get; set; }

        public string? JobProfileWebsiteUrl { get; set; }

        public List<IDysacContentModel>? GetContentItems()
        {
            return Skills.Select(x => (IDysacContentModel)x).ToList();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var skill in Skills.ToList())
            {
                if (skill.ItemId == contentItemId)
                {
                    Skills!.Remove(skill);
                }
            }
        }

        public IEnumerable<DysacSkillContentItemModel> SkillsToCompare(HashSet<string> skillsToRemove)
        {
            return Skills
                .OrderBy(x => x.Ordinal)
                .Where(skill => skill.AttributeType?.ToLower().Equals("knowledge", StringComparison.InvariantCultureIgnoreCase) != true)
                .Where(skill => !skillsToRemove.Contains(skill.Title!))
                .Take(8);
        }
    }
}
