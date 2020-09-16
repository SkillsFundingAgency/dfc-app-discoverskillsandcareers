using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    public class ApiQuestionSet : IBaseContentItemModel<ApiShortQuestion>
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__PrefLabel")]
        public string Title { get; set; }

        public string Type { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<ApiShortQuestion> ContentItems { get; set; } = new List<ApiShortQuestion>();

        private ContentLinksModel? PrivateLinksModel { get; set; }


    }
}
