namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISessionIdToCodeConverter
    {
        string GetCode(string value);

        string GetSessionId(string code);
    }
}
