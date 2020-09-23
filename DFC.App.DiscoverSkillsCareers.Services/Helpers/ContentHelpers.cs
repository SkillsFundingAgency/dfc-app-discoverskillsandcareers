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

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return new DysacQuestionSetContentModel();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalitySkill.ToUpperInvariant())
            {
                return new DysacSkill();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return new DysacTrait();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypeJobCategory.ToUpperInvariant())
            {
                return new JobCategory();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(ContentHelpers)}");
        }

        public static IBaseContentItemModel<ApiGenericChild> GetApiTypeFromContentType(string contentType)
        {
            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return new ApiQuestionSet();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalitySkill.ToUpperInvariant())
            {
                return new ApiSkill();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return new ApiTrait();
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypeJobCategory.ToUpperInvariant())
            {
                return new ApiJobCategory();
            }

            throw new InvalidOperationException($"{contentType} not supported in {nameof(ContentHelpers)}");
        }
    }
}
