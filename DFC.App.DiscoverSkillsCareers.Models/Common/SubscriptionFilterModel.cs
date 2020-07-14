﻿using Microsoft.Azure.Management.EventGrid.Models;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    public class SubscriptionFilterModel
    {
        public string BeginsWith { get; set; } = "/content/page/";

        public string? EndsWith { get; set; }

        public List<string> IncludeEventTypes { get; set; } = new List<string>();

        public StringInAdvancedFilter? PropertyContainsFilter { get; set; }
    }
}