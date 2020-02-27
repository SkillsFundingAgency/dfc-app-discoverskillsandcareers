namespace DFC.App.DiscoverSkillsCareers.Services
{
    public class SendEmailRequest
    {
        public string HostName { get; set; }

        public string EmailAddress { get; set; }

        public string TemplateId { get; set; }
    }
}