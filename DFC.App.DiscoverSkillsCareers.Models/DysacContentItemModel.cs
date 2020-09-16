using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DysacContentItemModel> ContentItems { get; set; } = new List<DysacContentItemModel>();
    }
}
