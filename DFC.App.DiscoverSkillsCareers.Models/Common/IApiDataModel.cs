using Newtonsoft.Json;
using System;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
