using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacTrait
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DysacShortQuestion> ContentItems { get; set; } = new List<DysacShortQuestion>();
    }
}
