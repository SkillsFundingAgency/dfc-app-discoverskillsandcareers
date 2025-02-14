﻿using DFC.App.DiscoverSkillsCareers.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac.PersonalityTrait;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultIndexResponseViewModel
    {
        public ResultIndexResponseViewModel()
        {
            Results = new ResultsIndexResponseViewModel();
        }

        public string AssessmentReference { get; set; }

        public ResultsIndexResponseViewModel Results { get; set; }

        public string AssessmentType { get; set; }

        public string SpeakToAnAdviser { get; set; }
    }
}
