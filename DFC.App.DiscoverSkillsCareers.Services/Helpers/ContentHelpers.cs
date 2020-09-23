using DFC.App.DiscoverSkillsCareers.Models.Contracts;
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

        public static void RemoveContentItem(Guid contentItemId, List<IDysacContentModel>? items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == contentItemId)
                {
                    items.Remove(contentItemModel);
                    return;
                }

                RemoveContentItem(contentItemId, contentItemModel.GetContentItems());
            }
        }
    }
}
