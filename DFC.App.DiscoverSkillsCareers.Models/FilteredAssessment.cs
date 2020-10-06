using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class FilteredAssessment
    {
        public string JobCategory { get; set; }
        public List<FilteredAssessmentQuestion> AssessmentFilteredQuestions { get; set; }
    }
}
