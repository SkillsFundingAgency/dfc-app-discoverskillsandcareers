using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacJobProfileOverviewContentModel : DocumentModel, IDocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "JobProfileOverview";

        public Uri? Url { get; set; }

        public string? Html { get; set; }

        public List<Guid>? AllContentItemIds => throw new NotImplementedException();

        public DateTime? LastCached { get; set; }

        public int? Ordinal { get; set; }

        public List<IDysacContentModel>? GetContentItems()
        {
            throw new NotImplementedException();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            throw new NotImplementedException();
        }
    }
}
