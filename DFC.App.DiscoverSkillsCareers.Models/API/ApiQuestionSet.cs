using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.API
{
    [ExcludeFromCodeCoverage]
    public class ApiQuestionSet : BaseContentItemModel, IBaseContentItemModel<ApiGenericChild>
    {
        public string? Type { get; set; }

        public IList<ApiGenericChild> ContentItems { get; set; } = new List<ApiGenericChild>();
    }
}
