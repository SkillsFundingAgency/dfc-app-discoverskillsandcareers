using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Validation
{
    public class ReferenceCodeExists : ValidationAttribute
    {
        private readonly HttpContext _httpContext;

        public ReferenceCodeExists(IHttpContextAccessor contextAccessor)
        {
            _httpContext = contextAccessor.HttpContext;
        }

        public ReferenceCodeExists()
        {
            
        }

        public override bool IsValid(object value)
        {
            var services = this._httpContext.RequestServices;
            var assessment = (IAssessmentService)services.GetService(typeof(IAssessmentService));

            return assessment.ReferenceCodeExists(value.ToString());
        }
    }
}
