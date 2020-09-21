using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiGenericChild : IBaseContentItemModel<ApiGenericChild>
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__PrefLabel")]
        public string? Title { get; set; }

        public string? Impact { get; set; }

        public string? Type { get; set; }

        public string? WebsiteURI { get; set; }

        public int Ordinal { get; set; }

        public string? ContentType { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<ApiGenericChild>? ContentItems { get; set; } = new List<ApiGenericChild>();

        public string? Description { get; set; }

        private ContentLinksModel? PrivateLinksModel { get; set; }
    }
}
