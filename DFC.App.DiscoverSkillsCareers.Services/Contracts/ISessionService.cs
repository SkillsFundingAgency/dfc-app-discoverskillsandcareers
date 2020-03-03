namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISessionService
    {
        T GetValue<T>(string key);

        void SetValue(string key, object value);
    }
}
