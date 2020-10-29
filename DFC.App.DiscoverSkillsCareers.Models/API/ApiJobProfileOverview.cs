using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiJobProfileOverview : IBaseContentItemModel
    {
        public ApiJobProfileOverview()
        {
            ContentItems = new List<IBaseContentItemModel>();
        }

        public ContentLinksModel? ContentLinks { get; set; }

        public IList<IBaseContentItemModel> ContentItems { get; set; }

        public Guid? ItemId { get; set; }

        public string? ContentType { get; set; }

        public string? Html { get; set; }

        public Uri? Url { get; set; }
    }
}
