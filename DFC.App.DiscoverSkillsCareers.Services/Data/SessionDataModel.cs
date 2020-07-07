namespace DFC.App.DiscoverSkillsCareers.Services.Data
{
    public class SessionDataModel
    {
        public string ApplicationName { get; set; }

        public string Salt { get; set; } = "ncs";

        public string DysacSessionId { get; set; }
    }
}
