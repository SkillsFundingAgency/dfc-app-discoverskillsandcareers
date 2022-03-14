using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacSkillContentItemModel : IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        public Uri? Url { get; set; }

        public string? Title { get; set; }

        public int? Ordinal { get; set; }

        public string? AttributeType { get; set; }

        public DateTime? LastCached { get; set; }

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public decimal? ONetRank { get; set; }

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
