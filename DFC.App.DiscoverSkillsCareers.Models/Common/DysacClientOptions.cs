using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    [ExcludeFromCodeCoverage]
    public class DysacClientOptions : ClientOptionsModel
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public Uri AssessmentApiBaseAddress { get; set; }

        public Uri ResultsApiBaseAddress { get; set; }

        public Uri QuestionApiBaseAddress { get; set; }

        public string OcpApimSubscriptionKey { get; set; }
    }
}
