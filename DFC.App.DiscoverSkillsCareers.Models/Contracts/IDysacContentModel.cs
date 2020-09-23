using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IDysacContentModel : IDocumentModel
    {
        string ContentType { get; set; }

        Uri Url { get; set; }

        List<Guid>? AllContentItemIds { get; }

        DateTime? LastCached { get; set; }

        Guid? ItemId { get; }

        List<IDysacContentModel>? GetContentItems();

        void RemoveContentItem(Guid contentItemId);
    }
}
