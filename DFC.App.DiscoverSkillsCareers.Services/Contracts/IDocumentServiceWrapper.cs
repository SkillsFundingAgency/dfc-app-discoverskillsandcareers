using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDocumentServiceFactory
    {
        object GetDocumentService<TModel>()
            where TModel : class, IDocumentModel;

    }
}
