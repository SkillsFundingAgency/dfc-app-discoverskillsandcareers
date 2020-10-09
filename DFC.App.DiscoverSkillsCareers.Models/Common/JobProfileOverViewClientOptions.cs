using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    [ExcludeFromCodeCoverage]
    public class JobProfileOverViewClientOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public Uri? BaseAddress { get; set; }
    }
}
