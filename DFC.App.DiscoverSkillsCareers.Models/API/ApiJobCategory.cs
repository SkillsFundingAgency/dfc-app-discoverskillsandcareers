using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    public class ApiJobCategory : BaseContentItemModel, IBaseContentItemModel<ApiGenericChild>
    {
        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public new IList<ApiGenericChild> ContentItems { get; set; } = new List<ApiGenericChild>();
    }
}
