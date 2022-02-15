using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class ContentHelpers
    {
        public static IDysacContentModel? FindContentItem(Guid contentItemId, List<IDysacContentModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == contentItemId)
                {
                    return contentItemModel;
                }

                var childContentItemModel = FindContentItem(contentItemId, contentItemModel.GetContentItems());

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }

        public static IDysacContentModel GetDsyacTypeFromContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException(contentType);
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return new DysacQuestionSetContentModel();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypeONetSkill.ToUpperInvariant())
            {
                return new DysacSkillContentModel();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return new DysacTraitContentModel();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypeJobCategory.ToUpperInvariant())
            {
                return new JobCategoryContentItemModel();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityShortQuestion.ToUpperInvariant())
            {
                return new DysacShortQuestionContentItemModel();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityFilteringQuestion.ToUpperInvariant())
            {
                return new DysacFilteringQuestionContentModel();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(ContentHelpers)}");
        }

        public static IBaseContentItemModel GetApiTypeFromContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return new ApiQuestionSet();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypeONetSkill.ToUpperInvariant())
            {
                return new ApiSkill();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return new ApiTrait();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypeJobCategory.ToUpperInvariant())
            {
                return new ApiJobCategory();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityShortQuestion.ToUpperInvariant())
            {
                return new ApiShortQuestion();
            }

            if (contentType.ToUpperInvariant() == DysacConstants.ContentTypePersonalityFilteringQuestion.ToUpperInvariant())
            {
                return new ApiPersonalityFilteringQuestion();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(ContentHelpers)}");
        }
    }
}
