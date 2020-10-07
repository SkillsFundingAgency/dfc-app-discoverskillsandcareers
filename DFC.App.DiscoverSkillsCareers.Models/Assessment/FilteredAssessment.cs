using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class FilteredAssessment
    {
        public FilteredAssessment()
        {
            this.JobCategoryAssessments = new List<JobCategoryAssessment>();
        }

        public List<JobCategoryAssessment> JobCategoryAssessments { get; set; }

        public IEnumerable<FilteredAssessmentQuestion>? Questions { get; set; }
    }
}
