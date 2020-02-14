namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDataProcessor<in T>
    {
        void Processor(T value);
    }
}
