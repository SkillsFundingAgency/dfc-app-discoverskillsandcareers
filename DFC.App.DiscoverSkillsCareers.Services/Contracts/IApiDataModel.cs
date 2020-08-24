using Newtonsoft.Json;
using System;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
