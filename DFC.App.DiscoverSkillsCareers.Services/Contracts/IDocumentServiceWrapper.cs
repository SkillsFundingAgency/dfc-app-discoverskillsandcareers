using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDocumentServiceWrapper
    {
        object GetDocumentService<TModel>()
            where TModel : class, IDocumentModel;

    }
}
