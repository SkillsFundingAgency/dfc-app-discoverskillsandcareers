using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class FilteredAssessment
    {
        public FilteredAssessment()
        {
            JobCategoryAssessments = new List<JobCategoryAssessment>();
        }

        public List<JobCategoryAssessment> JobCategoryAssessments { get; set; }

        public IEnumerable<FilteredAssessmentQuestion>? Questions { get; set; }

        public string? CurrentFilterAssessmentCode { get; set; }
    }
}
