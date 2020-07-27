using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    [Trait("Category", "Cache Reload background service Unit Tests")]
    public class CacheReloadServiceTests
    {
        private readonly ILogger<CacheReloadService> fakeLogger = A.Fake<ILogger<CacheReloadService>>();
        private readonly AutoMapper.IMapper fakeMapper = A.Fake<AutoMapper.IMapper>();
        private readonly CmsApiClientOptions fakeCmsApiClientOptions = A.Dummy<CmsApiClientOptions>();
        private readonly IEventMessageService<ContentItemModel> fakeEventMessageService = A.Fake<IEventMessageService<ContentItemModel>>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IContentCacheService fakeContentCacheService = A.Fake<IContentCacheService>();

        public CacheReloadServiceTests()
        {
            fakeCmsApiClientOptions.BaseAddress = new Uri("http://somewhere.com");
            fakeCmsApiClientOptions.SummaryEndpoint = "summary";
        }

        public static IEnumerable<object[]> TestValidationData => new List<object[]>
        {
            new object[] { BuildValidContentPageModel(), true },
            new object[] { A.Fake<ContentPageModel>(), false },
        };

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForCreate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeContentCacheService.Clear());
            A.CallTo(() => fakeMapper.Map<ContentItemModel>(A<ContentItemModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentCacheService.Clear()).MustHaveHappenedOnceExactly();
           // A.CallTo(() => fakeMapper.Map<ContentItemModel>(A<PagesApiDataModel>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForUpdate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeContentCacheService.Clear());
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentCacheService.Clear()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.GetAllCachedCanonicalNamesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForCreate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidPagesSummaryItemModel();
            var expectedValidPagesApiDataModel = BuildValidPagesApiDataModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();

            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService);

            // act
            await cacheReloadService.GetAndSaveItemAsync(expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForUpdate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidPagesSummaryItemModel();
            var expectedValidPagesApiDataModel = BuildValidPagesApiDataModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();

            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService);

            // act
            await cacheReloadService.GetAndSaveItemAsync(expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<PagesApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        private static ContentItemModel BuildValidPagesSummaryItemModel()
        {
            var model = new ContentItemModel()
            {
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                Published = DateTime.Now,
                CreatedDate = DateTime.Now,
            };

            return model;
        }

        private static PagesApiDataModel BuildValidPagesApiDataModel()
        {
            var model = new PagesApiDataModel()
            {
                ItemId = Guid.NewGuid(),
                CanonicalName = "an-article",
                Version = Guid.NewGuid(),
                BreadcrumbTitle = "An article",
                ExcludeFromSitemap = true,
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                RedirectLocations = "alt-name-1\r\nalt-name-2",
                Title = "A title",
                Description = "a description",
                Keywords = "some keywords",
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
                ContentItems = new List<PagesApiContentItemModel>
                {
                    new PagesApiContentItemModel { Alignment = "Left", Ordinal = 1, Size = 50, Content = "<h1>A document</h1>", },
                },
                Published = DateTime.UtcNow,
            };

            return model;
        }

        private static ContentItemModel BuildValidContentPageModel()
        {
            var model = new ContentItemModel()
            {
                Id = Guid.NewGuid().ToString(),
                CanonicalName = "an-article",
                BreadcrumbTitle = "An article",
                IncludeInSitemap = true,
                Version = Guid.NewGuid(),
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                Content = "<h1>A document</h1>",
                LastReviewed = DateTime.UtcNow,
            };

            return model;
        }

        private List<PagesSummaryItemModel> BuldFakePagesSummaryItemModel(int iemCount)
        {
            var models = A.CollectionOfFake<PagesSummaryItemModel>(iemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
                item.CanonicalName = id.ToString();
            }

            return models.ToList();
        }

        private List<ContentPageModel> BuldFakeContentPageModels(int iemCount)
        {
            var models = A.CollectionOfFake<ContentPageModel>(iemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Id = id;
                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
                item.CanonicalName = id.ToString();
            }

            return models.ToList();
        }
    }
}
