using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacSkill : DocumentModel, IDysacContentModel
    {   
        [Required]
        public override string? PartitionKey { get; set; } = "Skill";

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }
    
        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        private List<Guid>? GetAllContentItemIds()
        {
            return new List<Guid>();
        }
    }
}
