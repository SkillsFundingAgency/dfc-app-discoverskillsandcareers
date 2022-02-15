﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiSkill : BaseContentItemModel, IBaseContentItemModel
    {
        public string? Description { get; set; }

        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        public int? Ordinal { get; set; }

        [JsonProperty("ONetRank")]
        public decimal? ONetRank { get; set; }
    }
}
