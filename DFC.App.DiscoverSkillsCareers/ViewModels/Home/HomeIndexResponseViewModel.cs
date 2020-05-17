using DFC.App.DiscoverSkillsCareers.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class HomeIndexResponseViewModel
    {
        [Display(Name = "Reference Code")]
        [Required(ErrorMessage = "Enter your reference")]
        [ReferenceCodeExists(ErrorMessage = "The reference could not be found")]
        public string ReferenceCode { get; set; }
    }
}
