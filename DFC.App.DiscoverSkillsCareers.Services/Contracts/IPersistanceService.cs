namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IPersistanceService
    {
        string GetValue(string key);

        void SetValue(string key, string value);
    }
}
