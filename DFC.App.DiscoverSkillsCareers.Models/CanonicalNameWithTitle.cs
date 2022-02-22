namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class CanonicalNameWithTitle
    {
        public CanonicalNameWithTitle(string title, string canonicalName)
        {
            Title = title;
            CanonicalName = canonicalName;
        }

        public string Title { get; set; }

        public string CanonicalName { get; set; }
    }
}