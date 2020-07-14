namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    public class EventGridSubscriptionModel
    {
        public string? Name { get; set; }

        public string? Endpoint { get; set; }

        public SubscriptionFilterModel Filter { get; set; } = new SubscriptionFilterModel();
    }
}
