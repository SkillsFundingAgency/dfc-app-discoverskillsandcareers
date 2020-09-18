using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Telemetry.Models;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IDysacContentModel : IDocumentModel
    {
        string Title { get; set; }

        Uri Url { get; set; }

        List<Guid>? AllContentItemIds { get; }
    }
}
