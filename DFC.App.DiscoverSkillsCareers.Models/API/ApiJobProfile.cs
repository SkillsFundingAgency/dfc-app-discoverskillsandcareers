using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    public class ApiJobProfile : BaseContentItemModel, IBaseContentItemModel
    {
        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        public string? JobProfileWebsiteUrl { get; set; }

        public int? Ordinal { get; set; }
    }
}
