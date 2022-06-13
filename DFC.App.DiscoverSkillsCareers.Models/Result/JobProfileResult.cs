using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class JobProfileResult
    {
        [JsonIgnore]
        public string? JobCategory { get; set; }

        public string? Title { get; set; }

        public List<string>? SkillCodes { get; set; }
    }
}