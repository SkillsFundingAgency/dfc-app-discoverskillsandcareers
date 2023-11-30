using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ICachedModel
    {
        string? Title { get; set; }

        Uri? Url { get; set; }
    }
}
