using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategory : DocumentModel, IDysacContentModel
    {
        public string? Title { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "JobCategory";

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
