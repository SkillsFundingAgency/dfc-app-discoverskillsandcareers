using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategory
    {
        public string? Title { get; set; }

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? WebsiteURI { get; set; }

        public string? Description { get; set; }
    }
}
