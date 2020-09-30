using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiShortQuestion : BaseContentItemModel, IBaseContentItemModel<ApiGenericChild>
    {
        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public new IList<ApiGenericChild> ContentItems { get; set; } = new List<ApiGenericChild>();
    }
}