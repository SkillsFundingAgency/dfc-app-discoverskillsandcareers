using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentService<DysacTraitContentModel> dysacTraitDocumentService;
        private readonly IDocumentService<DysacSkillContentModel> dysacSkillDocumentService;

        public DocumentServiceFactory(IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService, IDocumentService<DysacTraitContentModel> dysacTraitDocumentService, IDocumentService<DysacSkillContentModel> dysacSkillDocumentService)
        {
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.dysacTraitDocumentService = dysacTraitDocumentService;
            this.dysacSkillDocumentService = dysacSkillDocumentService;
        }

        public IDocumentService<TModel> GetDocumentService<TModel>()
            where TModel : class, IDocumentModel
        {
            if (typeof(TModel) == typeof(DysacQuestionSetContentModel))
            {
                return (IDocumentService<TModel>)dysacQuestionSetDocumentService;
            }

            if (typeof(TModel) == typeof(DysacTraitContentModel))
            {
                return (IDocumentService<TModel>)dysacTraitDocumentService;
            }

            if (typeof(TModel) == typeof(DysacSkillContentModel))
            {
                return (IDocumentService<TModel>)dysacSkillDocumentService;
            }

            throw new InvalidOperationException($"No document service for {typeof(TModel)} found");
        }
    }
}
