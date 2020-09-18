using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class DocumentServiceWrapper : IDocumentServiceWrapper
    {
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentService<DysacTrait> dysacTraitDocumentService;

        public DocumentServiceWrapper(IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService, IDocumentService<DysacTrait> dysacTraitDocumentService)
        {
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.dysacTraitDocumentService = dysacTraitDocumentService;
        }

        public object GetDocumentService<TModel>()
            where TModel : class, IDocumentModel
        {
            if (typeof(TModel) == typeof(DysacQuestionSetContentModel))
            {
                return dysacQuestionSetDocumentService;
            }

            if (typeof(TModel) == typeof(DysacTrait))
            {
                return dysacTraitDocumentService;
            }

            throw new InvalidOperationException($"No document service for {typeof(TModel)} found");
        }
    }
}
