using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Cosmos.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.WebhooksServiceTests
{
    public abstract class BaseWebhooksServiceTests
    {
        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";

        protected BaseWebhooksServiceTests()
        {
            Logger = A.Fake<ILogger<WebhooksService>>();
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            FakeCmsApiService = A.Fake<ICmsApiService>();
            FakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
            FakeContentCacheService = A.Fake<IContentCacheService>();
            FakeContentProcessors = A.CollectionOfFake<IContentProcessor>(3);
        }

        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForDelete { get; } = Guid.NewGuid();

        protected ILogger<WebhooksService> Logger { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected ICmsApiService FakeCmsApiService { get; }

        protected IContentPageService<ContentPageModel> FakeContentPageService { get; }

        protected IContentCacheService FakeContentCacheService { get; }

        public IList<IContentProcessor> FakeContentProcessors { get; }

        protected static ApiQuestionSet BuildValidPagesApiContentModel()
        {
            var model = new ApiQuestionSet
            {
                ItemId = Guid.NewGuid(),
                Type = "Short",
                Url = new Uri("https://localhost"),
                ContentLinks = new ContentLinksModel(new JObject())
                {
                    ContentLinks = new List<KeyValuePair<string, List<LinkDetails>>>()
                    {
                        new KeyValuePair<string, List<LinkDetails>>(
                            "test",
                            new List<LinkDetails>
                            {
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.one.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.two.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.three.com"),
                                },
                            }),
                    },
                },
                ContentItems = new List<IBaseContentItemModel>
                {
                    BuildValidPagesApiContentItemDataModel(),
                },
                Published = DateTime.UtcNow,
            };

            return model;
        }

        protected static ApiShortQuestion BuildValidPagesApiContentItemDataModel()
        {
            var model = new ApiShortQuestion
            {
                Ordinal = 1,
                Title = "A Short question",
                Impact = "Positive",
                Description = "The question is..."

            };

            return model;
        }

        protected DysacQuestionSetContentModel BuildValidContentPageModel(string? contentType = null)
        {
            var model = new DysacQuestionSetContentModel()
            {
                Id = ContentIdForUpdate,
                Etag = Guid.NewGuid().ToString(),
                Url = new Uri("https://localhost"),
                ShortQuestions = new List<IDysacContentModel>
                {
                    BuildValidContentItemModel(ContentItemIdForCreate),
                    BuildValidContentItemModel(ContentItemIdForUpdate, contentType),
                    BuildValidContentItemModel(ContentItemIdForDelete),
                }
            };

            return model;
        }

        protected DysacShortQuestionContentItemModel BuildValidContentItemModel(Guid contentItemId, string? contentType = null)
        {
            var model = new DysacShortQuestionContentItemModel()
            {
                ItemId = contentItemId,
                Impact = "Positive",
                Title = "A Question..."
            };

            return model;
        }

        protected WebhooksService BuildWebhooksService()
        {
            SetupContentProcessors();
            var service = new WebhooksService(Logger, FakeCmsApiService, FakeContentCacheService, FakeContentProcessors);

            return service;
        }

        protected void SetupContentProcessors()
        {
            FakeContentProcessors[0] = A.Fake<IContentProcessor>();
            A.CallTo(() => FakeContentProcessors[0].Type).Returns(nameof(DysacQuestionSetContentModel));
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[0].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[0].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[0].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<IBaseContentItemModel>.Ignored)).Returns(System.Net.HttpStatusCode.OK);

            FakeContentProcessors[1] = A.Fake<IContentProcessor>();
            A.CallTo(() => FakeContentProcessors[1].Type).Returns(nameof(DysacTraitContentModel));
            A.CallTo(() => FakeContentProcessors[1].DeleteContentAsync(A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[1].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[1].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[1].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<IBaseContentItemModel>.Ignored)).Returns(System.Net.HttpStatusCode.OK);

            FakeContentProcessors[2] = A.Fake<IContentProcessor>();
            A.CallTo(() => FakeContentProcessors[2].Type).Returns(nameof(DysacSkillContentModel));
            A.CallTo(() => FakeContentProcessors[2].DeleteContentAsync(A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[2].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[2].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => FakeContentProcessors[2].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<IBaseContentItemModel>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
        }
    }
}