using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class JobCategory
    {
        public string Title { get; set; }

        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string WebsiteURI { get; set; }

        public string Description { get; set; }
    }
}
