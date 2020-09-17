using DFC.App.DiscoverSkillsCareers.Models.Extensions;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class DysacQuestionSetContentModel : DocumentModel, IDocumentModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [Required]
        public override string? PartitionKey { get; set; } = "QuestionSet";

        public List<DysacShortQuestion>? ShortQuestions { get; set; } = new List<DysacShortQuestion>();

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => ShortQuestions.Select(z => z.ItemId!.Value).Union(ShortQuestions.SelectMany(z => z.Traits.Select(y => y.ItemId!.Value))).ToList();

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Type { get; set; }
    }
}
