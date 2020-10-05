using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Converters;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacTraitContentModel : DocumentModel, IDocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "Trait";

        public Uri? Url { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }

        public int? Ordinal { get; set; }

        public DateTime? LastCached { get; set; }

        public List<JobCategoryContentItemModel> JobCategories { get; set; } = new List<JobCategoryContentItemModel>();

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public List<IDysacContentModel>? GetContentItems()
        {
            return JobCategories.Select(x => (IDysacContentModel)x).ToList(); ;
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var jobCategory in JobCategories.ToList())
            {
                if (jobCategory.ItemId == contentItemId)
                {
                    JobCategories.Remove(jobCategory);
                }
            }
        }

        private List<Guid>? GetAllContentItemIds()
        {
            return JobCategories.Select(z => z.ItemId!.Value).ToList();
        }
    }
}
