using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static NuGet.Client.ManagedCodeConventions;

namespace DFC.App.DiscoverSkillsCareers.Validation
{
    [ExcludeFromCodeCoverage]
    public class RequiredWhenSelectedAttribute : ValidationAttribute
    {
        /// <summary>
        /// Values of the <see cref="PropertyName"/> that will trigger the validation
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        /// PropertyName
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;
            if (model == null || Values == null)
            {
                return ValidationResult.Success;
            }

            var currentValue = model.GetType().GetProperty(PropertyName)?.GetValue(model, null)?.ToString();
            if (Values.Contains(currentValue) && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
