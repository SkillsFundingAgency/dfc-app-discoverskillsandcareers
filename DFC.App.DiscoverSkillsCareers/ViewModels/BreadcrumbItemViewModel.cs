using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.SkillsHealthCheck.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbItemViewModel
    {
        public string? Route { get; set; }

        public string? Title { get; set; }

        [JsonIgnore]
        public bool AddHyperlink { get; set; } = true;
    }
}
