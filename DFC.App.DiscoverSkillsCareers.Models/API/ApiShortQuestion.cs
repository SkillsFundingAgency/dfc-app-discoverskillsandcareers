using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    public class ApiShortQuestion : IBaseContentItemModel<ApiShortQuestion>
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__PrefLabel")]
        public string Title { get; set; }

        public string Impact { get; set; }

        public int Ordinal { get; set; }

        public ContentLinksModel? ContentLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<ApiShortQuestion> ContentItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
