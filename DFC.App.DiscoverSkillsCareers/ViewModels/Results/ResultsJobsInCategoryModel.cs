﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultsJobsInCategoryModel
    {
        public string CategoryUrl { get; set; }

        public string CategoryTitle { get; set; }

        public string CategoryCode { get; set; }

        public int NumberOfSuitableRoles { get; set; }

        public int DisplayOrder { get; set; }

        public bool ShowThisCategory { get; set; }

        public int AnswerMoreQuestions { get; set; }

        public IEnumerable<ResultJobProfileOverViewModel> JobProfiles { get; set; }
    }
}