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
    public class DysacShortQuestion : DocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "ShortQuestion";

        public string? Impact { get; set; }

        public string? Title { get; set; }

        public string? ContentType { get; set; }

        public Uri? Url { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<DysacTrait>))]
        public List<IDysacContentModel> Traits { get; set; } = new List<IDysacContentModel>();

        public DateTime? LastCached { get; set; }

        public List<Guid>? AllContentItemIds => new List<Guid>();

        public List<IDysacContentModel>? GetContentItems()
        {
            return Traits.Union(Traits.SelectMany(x => x.GetContentItems())).ToList();
        }
    }
}
