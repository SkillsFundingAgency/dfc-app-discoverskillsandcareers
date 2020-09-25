using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacSkillContentModel : DocumentModel, IDocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "Skill";

        public Uri? Url { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }

        public int? Ordinal { get; set; }

        public DateTime? LastCached { get; set; }

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public List<IDysacContentModel>? GetContentItems()
        {
            return new List<IDysacContentModel>();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            throw new NotImplementedException();
        }

        private List<Guid>? GetAllContentItemIds()
        {
            return new List<Guid>();
        }
    }
}
