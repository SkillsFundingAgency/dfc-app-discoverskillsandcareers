﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentReferenceGetResponse
    {
        public string ReferenceCode { get; set; }

        public string AssessmentStarted { get; set; }

        [Required(ErrorMessage = "Enter a phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }
    }
}
