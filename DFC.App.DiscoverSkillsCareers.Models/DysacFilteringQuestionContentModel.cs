using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Converters;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacFilteringQuestionContentModel : DocumentModel, IDocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "FilteringQuestion";

        public Uri? Url { get; set; }

        public string? Text { get; set; }

        public string? Title { get; set; }

        public DateTime? LastCached { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<DysacSkillContentItemModel>))]
        public List<IDysacContentModel> Skills { get; set; } = new List<IDysacContentModel>();

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public int? Ordinal { get; set; }

        public List<IDysacContentModel>? GetContentItems()
        {
            return Skills;
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            foreach (var skill in Skills.ToList())
            {
                if (skill.ItemId == contentItemId)
                {
                    Skills.Remove(skill);
                }
            }
        }

        private List<Guid>? GetAllContentItemIds()
        {
            return Skills.Select(z => z.ItemId!.Value).ToList();
        }
    }
}