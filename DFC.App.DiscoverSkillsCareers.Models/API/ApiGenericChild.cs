using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiGenericChild : BaseContentItemModel, IBaseContentItemModel<ApiGenericChild>
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Impact { get; set; }

        public string? Type { get; set; }

        public string? WebsiteURI { get; set; }

        public int Ordinal { get; set; }

        public new IList<ApiGenericChild> ContentItems { get; set; } = new List<ApiGenericChild>();
    }
}
