namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface ISerialiser
    {
        string Serialise(object value);

        T Deserialise<T>(string value);
    }
}
