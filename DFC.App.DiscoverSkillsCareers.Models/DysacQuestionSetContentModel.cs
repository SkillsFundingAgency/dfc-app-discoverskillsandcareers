using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Converters;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacQuestionSetContentModel : DocumentModel, IDysacContentModel
    {
        public Guid? ItemId { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "QuestionSet";

        [JsonConverter(typeof(ConcreteTypeConverter<DysacShortQuestion>))]
        public List<IDysacContentModel>? ShortQuestions { get; set; } = new List<IDysacContentModel>();

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        public string? ContentType { get; set; }

        [Required]
        public string? Type { get; set; }

        public DateTime? LastCached { get; set; }

        public List<IDysacContentModel>? GetContentItems()
        {
            return ShortQuestions!.ToList();
        }

        private List<Guid>? GetAllContentItemIds()
        {
            return ShortQuestions.Select(z => z.ItemId!.Value).Union(ShortQuestions.SelectMany(z => z.GetContentItems().Select(y => y.ItemId!.Value))).ToList();
        }
    }
}
