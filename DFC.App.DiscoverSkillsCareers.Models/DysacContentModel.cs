using DFC.App.DiscoverSkillsCareers.Models.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacContentModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        public override string? PartitionKey => PageLocation;

        public List<DysacContentItemModel> ContentItems { get; set; } = new List<DysacContentItemModel>();

        [JsonIgnore]
        public List<Guid> AllContentItemIds => ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value).ToList();

        public override string PageLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }
}
