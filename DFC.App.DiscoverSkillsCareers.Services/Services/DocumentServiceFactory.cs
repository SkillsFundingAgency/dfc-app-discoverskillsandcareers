using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentService<DysacTrait> dysacTraitDocumentService;
        private readonly IDocumentService<DysacSkill> dysacSkillDocumentService;

        public DocumentServiceFactory(IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService, IDocumentService<DysacTrait> dysacTraitDocumentService, IDocumentService<DysacSkill> dysacSkillDocumentService)
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

            if (typeof(TModel) == typeof(DysacTrait))
            {
                return (IDocumentService<TModel>)dysacTraitDocumentService;
            }

            if (typeof(TModel) == typeof(DysacSkill))
            {
                return (IDocumentService<TModel>)dysacSkillDocumentService;
            }

            throw new InvalidOperationException($"No document service for {typeof(TModel)} found");
        }

        public IDocumentService<TModel> GetDocumentService<TModel>(string contentType)
             where TModel : class, IDocumentModel
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityQuestionSet.ToUpperInvariant())
            {
                return (IDocumentService<TModel>)dysacQuestionSetDocumentService;
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalitySkill.ToUpperInvariant())
            {
                return (IDocumentService<TModel>)dysacSkillDocumentService;
            }

            if (contentType.ToUpperInvariant() == Constants.ContentTypePersonalityTrait.ToUpperInvariant())
            {
                return (IDocumentService<TModel>)dysacTraitDocumentService;
            }

            throw new InvalidOperationException($"No document service for {contentType} found");
        }
    }
}
