using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    public class ApiJobProfileOverview : IBaseContentItemModel
    {
        public ContentLinksModel? ContentLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<IBaseContentItemModel> ContentItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid? ItemId { get; set; }

        public string? ContentType { get; set; }

        public Uri? Url { get; set; }
    }
}
