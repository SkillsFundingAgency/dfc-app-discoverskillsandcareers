using System;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class ShortQuestion
    {
        public Guid? Id { get; set; }

        public string? Text { get; set; }

        public bool IsComplete { get; set; }

        public bool IsNegative { get; set; }

        public string? Answer { get; set; }

        public int? Ordinal { get; set; }

        public string? TraitCode { get; set; }
    }
}
