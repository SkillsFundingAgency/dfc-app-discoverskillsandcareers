using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IDocumentServiceFactory
    {
        IDocumentService<TModel> GetDocumentService<TModel>()
            where TModel : class, IDocumentModel;

        IDocumentService<TModel> GetDocumentService<TModel>(string contentType)
           where TModel : class, IDocumentModel;

    }
}
