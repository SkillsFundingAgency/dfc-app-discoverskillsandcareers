using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryContentItemModel : IDysacContentModel
    {
        public string? Title { get; set; }

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? WebsiteURI { get; set; }

        public string? Description { get; set; }

        public string? ContentType { get; set; }

        public DateTime? LastCached { get; set; }

        public List<Guid>? AllContentItemIds => new List<Guid>();

        public List<IDysacContentModel>? GetContentItems()
        {
            return new List<IDysacContentModel>();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
        }
    }
}
