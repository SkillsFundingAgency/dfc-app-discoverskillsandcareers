using DFC.App.DiscoverSkillsCareers.Models.Contracts;
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
    public class DysacJobProfileCategoryContentModel : DocumentModel, IDocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        public string? Title { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "JobProfileCategory";

        public List<JobProfileContentItemModel> JobProfiles { get; set; } = new List<JobProfileContentItemModel>();

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public DateTime? LastCached { get; set; }

        public int? Ordinal { get; set; }

        public List<IDysacContentModel>? GetContentItems()
        {
            return JobProfiles!.Select(x => (IDysacContentModel)x).ToList();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var jp in JobProfiles.ToList())
            {
                if (jp.ItemId == contentItemId)
                {
                    JobProfiles!.Remove(jp);
                }
            }
        }

        private List<Guid>? GetAllContentItemIds()
        {
            return JobProfiles.Select(z => z.ItemId!.Value).ToList();
        }
    }
}
