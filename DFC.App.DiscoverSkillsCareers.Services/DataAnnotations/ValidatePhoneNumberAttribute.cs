using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace DFC.App.DiscoverSkillsCareers.Services.DataAnnotations
{
    public class ValidatePhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Values of the <see cref="PropertyName"/> that will trigger the validation.
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        /// PropertyName.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Error Message.
        /// </summary>
        public string ErrorMessage { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;
            if (model == null || Values == null)
            {
                return ValidationResult.Success;
            }

            string? phoneNumber = value?.ToString();
            phoneNumber = phoneNumber?.Trim();

            var currentType = model.GetType();
            var currentValue = currentType.GetProperty(PropertyName)?.GetValue(model, null)?.ToString();
            if (Values.Contains(currentValue))
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return ValidationResult.Success;
                }

                var result = Regex.IsMatch(phoneNumber, @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                if (!result) { return new ValidationResult(ErrorMessage); }

                return ValidationResult.Success;
            }

            return ValidationResult.Success;
        }
    }
}
