using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiQuestionSet : BaseContentItemModel, IBaseContentItemModel
    {
        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();
    }
}
