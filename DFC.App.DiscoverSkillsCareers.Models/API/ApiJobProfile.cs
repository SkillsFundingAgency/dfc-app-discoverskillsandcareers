﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiJobProfile : BaseContentItemModel, IBaseContentItemModel
    {
        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        [JsonProperty("pagelocation_FullUrl")]
        public string? JobProfileWebsiteUrl { get; set; }

        public int? Ordinal { get; set; }
    }
}
