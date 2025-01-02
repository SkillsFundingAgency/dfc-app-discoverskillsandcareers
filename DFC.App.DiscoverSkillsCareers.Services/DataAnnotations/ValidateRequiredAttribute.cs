using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.DataAnnotations
{
    public class ValidateRequiredAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !string.IsNullOrEmpty(value?.ToString());
        }
    }
}
