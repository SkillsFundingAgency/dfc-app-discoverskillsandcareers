using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    [ExcludeFromCodeCoverage]
    public class HttpClientSettings
    {
        public TimeSpan Timeout { get; set; }

        public Uri BaseAddress { get; set; }

        public string OcpApimSubscriptionKey { get; set; }
    }
}
