using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class ContentDysacModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        public override string? PartitionKey => PageLocation;

        public List<ContentItemModel> ContentItems { get; set; } = new List<ContentItemModel>();

        [JsonIgnore]
        public List<Guid> AllContentItemIds => ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value).ToList();
    }
}
