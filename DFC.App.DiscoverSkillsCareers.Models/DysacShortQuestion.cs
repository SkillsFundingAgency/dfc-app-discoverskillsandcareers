using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacShortQuestion
    {
        public string? Impact { get; set; }

        public string? Title { get; set; }

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DysacTrait> Traits { get; set; } = new List<DysacTrait>();
    }
}
