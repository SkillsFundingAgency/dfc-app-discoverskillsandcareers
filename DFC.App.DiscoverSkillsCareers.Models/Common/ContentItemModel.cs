using Newtonsoft.Json;
using System;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    public class ContentItemModel : Compui.Cosmos.Models.ContentPageModel, IApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "htmlbody_Html")]
        public string? Html_Content { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public override string? PageLocation { get; set; } = "/shared-content";


    }
}
