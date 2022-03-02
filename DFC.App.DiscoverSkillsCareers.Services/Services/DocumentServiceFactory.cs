using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    [ExcludeFromCodeCoverage]
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentService<DysacTraitContentModel> dysacTraitDocumentService;
        private readonly IDocumentService<DysacSkillContentModel> dysacSkillDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService;
        private readonly IDocumentService<DysacJobProfileOverviewContentModel> dysacJobProfileDocumentService;
        private readonly IDocumentService<DysacJobProfileCategoryContentModel> dysacJobProfileCategoryDocumentService;

        public DocumentServiceFactory(
            IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService,
            IDocumentService<DysacTraitContentModel> dysacTraitDocumentService,
            IDocumentService<DysacSkillContentModel> dysacSkillDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService,
            IDocumentService<DysacJobProfileOverviewContentModel> dysacJobProfileDocumentService,
            IDocumentService<DysacJobProfileCategoryContentModel> dysacJobProfileCategoryDocumentService)
        {
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.dysacTraitDocumentService = dysacTraitDocumentService;
            this.dysacSkillDocumentService = dysacSkillDocumentService;
            this.dysacFilteringQuestionDocumentService = dysacFilteringQuestionDocumentService;
            this.dysacJobProfileDocumentService = dysacJobProfileDocumentService;
            this.dysacJobProfileCategoryDocumentService = dysacJobProfileCategoryDocumentService;
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

            if (typeof(TModel) == typeof(DysacFilteringQuestionContentModel))
            {
                return (IDocumentService<TModel>)dysacFilteringQuestionDocumentService;
            }

            if (typeof(TModel) == typeof(DysacJobProfileOverviewContentModel))
            {
                return (IDocumentService<TModel>)dysacJobProfileDocumentService;
            }

            if (typeof(TModel) == typeof(DysacJobProfileCategoryContentModel))
            {
                return (IDocumentService<TModel>)dysacJobProfileCategoryDocumentService;
            }

            throw new InvalidOperationException($"No document service for {typeof(TModel)} found");
        }
    }
}
