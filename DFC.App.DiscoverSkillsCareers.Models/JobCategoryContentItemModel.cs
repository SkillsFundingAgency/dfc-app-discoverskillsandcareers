using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryContentItemModel : IDysacContentModel
    {
        public string? Title { get; set; }

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? WebsiteURI { get; set; }

        public string? Description { get; set; }

        public DateTime? LastCached { get; set; }

        public List<Guid>? AllContentItemIds => new List<Guid>();

        public int? Ordinal { get; set; }

        public List<JobProfileContentItemModel> JobProfiles { get; set; } = new List<JobProfileContentItemModel>();

        public List<IDysacContentModel>? GetContentItems()
        {
            return JobProfiles!.Select(x => (IDysacContentModel)x).ToList();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var jp in JobProfiles.ToList())
            {
                if (jp.ItemId == contentItemId)
                {
                    JobProfiles!.Remove(jp);
                }
            }
        }
    }
}
