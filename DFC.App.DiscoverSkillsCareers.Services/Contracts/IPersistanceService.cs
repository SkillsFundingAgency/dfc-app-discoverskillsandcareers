namespace DFC.App.DiscoverSkillsCareers.Services
{
    public interface IPersistanceService
    {
        string GetValue(string key);

        void SetValue(string key, string value);
    }
}
