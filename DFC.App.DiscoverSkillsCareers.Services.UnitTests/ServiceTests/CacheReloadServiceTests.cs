using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
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
        private readonly IEventMessageService fakeEventMessageService = A.Fake<IEventMessageService>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IContentCacheService fakeContentCacheService = A.Fake<IContentCacheService>();
        private readonly IJobProfileOverviewApiService fakeJobProfileOverviewService = A.Fake<IJobProfileOverviewApiService>();

        public static IEnumerable<object[]> TestValidationData => new List<object[]>
        {
            new object[] { BuildValidDysacQuestionSetContentModel(), true },
            new object[] { A.Fake<DysacQuestionSetContentModel>(), false },
        };

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForCreate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidDysacQuestionSetContentModel = BuildValidDysacQuestionSetContentModel();
            var fakePagesSummaryItemModels = BuldFakeQuestionSetSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<ApiSummaryItemModel>(A<string>.Ignored)).Returns(fakePagesSummaryItemModels);
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(A.Fake<ApiQuestionSet>());
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(expectedValidDysacQuestionSetContentModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.Created);
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).Returns(fakeCachedDysacQuestionSetContentModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeJobProfileOverviewService.GetOverviews(A<List<string>>.Ignored)).Returns(new List<ApiJobProfileOverview>() { new ApiJobProfileOverview { CanonicalName = "A-Job-Profile" } });
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacJobProfileOverviewContentModel>()).Returns(new List<DysacJobProfileOverviewContentModel>() { new DysacJobProfileOverviewContentModel { Title = "A Job Profile" } });

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacJobProfileOverviewContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacJobProfileOverviewContentModel>(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForUpdate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidDysacQuestionSetContentModel = BuildValidDysacQuestionSetContentModel();
            var fakePagesSummaryItemModels = BuldFakeQuestionSetSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<ApiSummaryItemModel>(A<string>.Ignored)).Returns(fakePagesSummaryItemModels);
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(A.Fake<ApiQuestionSet>());
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(expectedValidDysacQuestionSetContentModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).Returns(fakeCachedDysacQuestionSetContentModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<ApiSummaryItemModel>(A<string>.Ignored)).MustHaveHappened(4, Times.Exactly);
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceReloadIsCancelled()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 1;
            var cancellationToken = new CancellationToken(true);
            var fakePagesSummaryItemModels = BuldFakeQuestionSetSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).Returns(fakeCachedDysacQuestionSetContentModels);
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<ApiSummaryItemModel>(A<string>.Ignored)).Returns(fakePagesSummaryItemModels);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<ApiSummaryItemModel>(A<string>.Ignored)).MustHaveHappened(4, Times.Exactly);
            A.CallTo(() => fakeContentCacheService.Clear()).MustNotHaveHappened();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }
        
        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForCreate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidSummaryModel();
            var expectedValidApiQuestionSet = BuildValidApiQuestionSet();
            var expectedValidDysacQuestionSetContentModel = BuildValidDysacQuestionSetContentModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(expectedValidApiQuestionSet);
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(expectedValidDysacQuestionSetContentModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
             A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.GetAndSaveItemAsync<ApiQuestionSet, DysacQuestionSetContentModel>(DysacConstants.ContentTypePersonalityQuestionSet, expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForUpdate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidSummaryModel();
            var expectedValidApiQuestionSet = BuildValidApiQuestionSet();
            var expectedValidDysacQuestionSetContentModel = BuildValidDysacQuestionSetContentModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(expectedValidApiQuestionSet);
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(expectedValidDysacQuestionSetContentModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.GetAndSaveItemAsync<ApiQuestionSet, DysacQuestionSetContentModel>(DysacConstants.ContentTypePersonalityQuestionSet, expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsCancelled()
        {
            // arrange
            var cancellationToken = new CancellationToken(true);
            var expectedValidPagesSummaryItemModel = BuildValidSummaryModel();
            var expectedValidApiQuestionSet = BuildValidApiQuestionSet();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(expectedValidApiQuestionSet);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.GetAndSaveItemAsync<ApiQuestionSet, DysacQuestionSetContentModel>(DysacConstants.ContentTypePersonalityQuestionSet, expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleCacheEntriesIsSuccessful()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var fakePagesSummaryItemModels = BuldFakeQuestionSetSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).Returns(fakeCachedDysacQuestionSetContentModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.DeleteStaleCacheEntriesAsync<DysacQuestionSetContentModel>(fakePagesSummaryItemModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleCacheEntriesIsCancelled()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(true);
            var fakePagesSummaryItemModels = BuldFakeQuestionSetSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).Returns(fakeCachedDysacQuestionSetContentModels);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.DeleteStaleCacheEntriesAsync<DysacQuestionSetContentModel>(fakePagesSummaryItemModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsSuccessful()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(false);
            var fakeDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeDysacQuestionSetContentModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsUnsuccessful()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(false);
            var fakeDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeDysacQuestionSetContentModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsCancelled()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(true);
            var fakeDysacQuestionSetContentModels = BuldFakeDysacQuestionSetContentModels(NumberOfDeletions);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeDysacQuestionSetContentModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [MemberData(nameof(TestValidationData))]
        public void CacheReloadServiceTryValidateModelForValidAndInvalid(DysacQuestionSetContentModel DysacQuestionSetContentModel, bool expectedResult)
        {
            // arrange
            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, A.Fake<IContentTypeMappingService>(), fakeJobProfileOverviewService);

            // act
            var result = cacheReloadService.TryValidateModel(DysacQuestionSetContentModel);

            // assert
            A.Equals(result, expectedResult);
        }

        private static ApiSummaryItemModel BuildValidSummaryModel()
        {
            var model = new ApiSummaryItemModel()
            {
                Title = "an-article",
                Url = new Uri("/aaa/bbb", UriKind.Relative)
            };

            return model;
        }

        private static ApiQuestionSet BuildValidApiQuestionSet()
        {
            var model = new ApiQuestionSet()
            {
                ItemId = Guid.NewGuid(),
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                ContentLinks = new ContentLinksModel(new JObject())
                {
                    ContentLinks = new List<KeyValuePair<string, List<ILinkDetails>>>()
                    {
                        new KeyValuePair<string, List<ILinkDetails>>(
                            "test",
                            new List<ILinkDetails>
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
                    new ApiShortQuestion { Ordinal = 1, Title = "Test Child" },
                }
            };

            return model;
        }

        private static DysacQuestionSetContentModel BuildValidDysacQuestionSetContentModel()
        {
            var model = new DysacQuestionSetContentModel()
            {
                Id = Guid.NewGuid(),
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                Type = "Short",
                ShortQuestions = new List<DysacShortQuestionContentItemModel>
                {
                    new DysacShortQuestionContentItemModel
                    {
                        ItemId = Guid.NewGuid(),
                        Title = "title",
                        Traits = new List<DysacTraitContentItemModel>
                        {
                            new DysacTraitContentItemModel
                            {   
                                Url = new Uri("http://www.test.com"),
                                ItemId = Guid.NewGuid(),
                                Description = "A test trait",
                                Title = "Trait 1"
                            },
                        },
                    },
                }
            };

            return model;
        }

        private List<ApiSummaryItemModel> BuldFakeQuestionSetSummaryItemModel(int iemCount)
        {
            var models = A.CollectionOfFake<ApiSummaryItemModel>(iemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
            }

            return models.ToList();
        }

        private List<DysacQuestionSetContentModel> BuldFakeDysacQuestionSetContentModels(int itemCount)
        {
            var models = A.CollectionOfFake<DysacQuestionSetContentModel>(itemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Id = id;
                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
            }

            return models.ToList();
        }
    }
}
