using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.Services.Processors
{
    public class BaseContentProcessor
    {
        private readonly ILogger<BaseContentProcessor> logger;

        public BaseContentProcessor(ILogger<BaseContentProcessor> logger)
        {
            this.logger = logger;
        }

        public bool TryValidateModel(IDysacContentModel? contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.Id} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
