using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiSkill : BaseContentItemModel, IBaseContentItemModel
    {
        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        public int? Ordinal { get; set; }

        public string? ONetRank { get; set; }

        public string? ONetAttributeType { get; set; }
    }
}
