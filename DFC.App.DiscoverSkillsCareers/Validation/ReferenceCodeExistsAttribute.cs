using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Validation
{
    [ExcludeFromCodeCoverage]
    public class ReferenceCodeExistsAttribute : ValidationAttribute
    {
        private readonly HttpContext httpContext;

        public ReferenceCodeExistsAttribute(IHttpContextAccessor contextAccessor)
        {
            httpContext = contextAccessor?.HttpContext;
        }

        public ReferenceCodeExistsAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Value cannot be null " + value);
            }

            var services = this.httpContext.RequestServices;
            var assessment = (IAssessmentService)services.GetService(typeof(IAssessmentService));

            return assessment.ReferenceCodeExists(value.ToString());
        }
    }
}
