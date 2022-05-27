using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiPersonalityFilteringQuestion : BaseContentItemModel, IBaseContentItemModel
    {
        public string? Text { get; set; }

        public int? Ordinal { get; set; }

        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();
    }
}
