using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentHelperTests
{
    [Trait("Category", "Content Helper Unit Tests")]
    public class ContentHelperTests
    {
        [Theory]
        [InlineData(DysacConstants.ContentTypePersonalityQuestionSet, typeof(DysacQuestionSetContentModel))]
        [InlineData(DysacConstants.ContentTypePersonalityTrait, typeof(DysacTraitContentModel))]
        [InlineData(DysacConstants.ContentTypeSkill, typeof(DysacSkillContentModel))]
        [InlineData(DysacConstants.ContentTypeJobCategory, typeof(JobCategoryContentItemModel))]
        [InlineData(DysacConstants.ContentTypePersonalityFilteringQuestion, typeof(DysacFilteringQuestionContentModel))]
        public void ContentHelpersGetDysacTypeFromContentTypeReturnsTrue(string contentType, Type type)
        {
            //Arrange
            //Act
            var result = ContentHelpers.GetDsyacTypeFromContentType(contentType);

            //Assert
            Assert.IsType(type, result);
        }

        [Fact]
        public void ContentHelpersGetDysacTypeFromContentTypeThrowsException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() => ContentHelpers.GetDsyacTypeFromContentType("randomcontenttype"));
        }

        [Theory]
        [InlineData(DysacConstants.ContentTypePersonalityQuestionSet, typeof(ApiQuestionSet))]
        [InlineData(DysacConstants.ContentTypePersonalityTrait, typeof(ApiTrait))]
        [InlineData(DysacConstants.ContentTypeSkill, typeof(ApiSkill))]
        [InlineData(DysacConstants.ContentTypeJobCategory, typeof(ApiJobCategory))]
        [InlineData(DysacConstants.ContentTypePersonalityFilteringQuestion, typeof(ApiPersonalityFilteringQuestion))]
        public void ContentHelpersGetApiTypeFromContentTypeReturnsTrue(string contentType, Type type)
        {
            //Arrange
            //Act
            var result = ContentHelpers.GetApiTypeFromContentType(contentType);

            //Assert
            Assert.IsType(type, result);
        }

        [Fact]
        public void ContentHelpersGetApiTypeFromContentTypeThrowsException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() => ContentHelpers.GetApiTypeFromContentType("randomcontenttype"));
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsSuccess()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var expectedContentItemModel = new DysacShortQuestionContentItemModel
            {
                ItemId = contentItemId,
            };
            var items = BuildContentItemSet(contentItemId);

            // Act
            var result = ContentHelpers.FindContentItem(contentItemId, items);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedContentItemModel.ItemId, result!.ItemId);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullforNotFound()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = BuildContentItemSet();

            // Act
            var result = ContentHelpers.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullForNullContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            List<IDysacContentModel>? items = null;

            // Act
            var result = ContentHelpers.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullForNNoContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = new List<IDysacContentModel>();

            // Act
            var result = ContentHelpers.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        private List<IDysacContentModel> BuildContentItemSet(Guid? contentId = null)
        {
            var items = new List<IDysacContentModel>
            {
                new DysacShortQuestionContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    Traits = new List<DysacTraitContentItemModel>
                    {
                        new DysacTraitContentItemModel
                        {
                            ItemId = contentId ?? Guid.NewGuid(),
                            JobCategories = new List<JobCategoryContentItemModel>
                            {
                                new JobCategoryContentItemModel
                                {
                                    ItemId = Guid.NewGuid(),
                                },
                            },
                        },
                    },
                },
            };

            return items;
        }
    }
}
