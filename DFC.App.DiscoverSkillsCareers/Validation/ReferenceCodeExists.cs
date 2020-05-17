using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Validation
{
    public class ReferenceCodeExists : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var assessmentService = DependencyResolver.Current.GetService<IAssessmentService>();

            return assessmentService.ReferenceCodeExists(value.ToString());
        }
    }
}
