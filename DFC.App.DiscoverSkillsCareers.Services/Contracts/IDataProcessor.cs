namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDataProcessor<T>
    {
        void Processor(T value);
    }
}
