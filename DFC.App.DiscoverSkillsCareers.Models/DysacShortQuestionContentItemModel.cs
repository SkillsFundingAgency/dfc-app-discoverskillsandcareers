using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacShortQuestionContentItemModel : IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        public string? Impact { get; set; }

        public int? Ordinal { get; set; }

        public string? Title { get; set; }

        public Uri? Url { get; set; }

        public List<DysacTraitContentItemModel> Traits { get; set; } = new List<DysacTraitContentItemModel>();

        public DateTime? LastCached { get; set; }

        public List<Guid>? AllContentItemIds => new List<Guid>();

        public List<IDysacContentModel>? GetContentItems()
        {
            return Traits.Union(Traits.SelectMany(x => x.GetContentItems())).ToList();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var trait in Traits.ToList())
            {
                trait.RemoveContentItem(contentItemId);

                if (trait.ItemId == contentItemId)
                {
                    Traits.Remove(trait);
                }
            }
        }
    }
}
