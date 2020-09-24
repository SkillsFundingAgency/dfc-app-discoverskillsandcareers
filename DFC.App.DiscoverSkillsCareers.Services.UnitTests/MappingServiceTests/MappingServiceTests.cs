using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.MappingServiceTests
{
    [Trait("Category", "Mapping Service Unit Tests")]
    public class MappingServiceTests
    {
        [Fact]
        public void MappingServiceWhenPassedDysacQuestionSetReturnsQuestionSet()
        {
            //Arrange
            var mappingService = new MappingService(A.Fake<IMapper>());

            //Act
            var result = mappingService.Map(new DysacShortQuestionContentItemModel(), new ApiGenericChild());

            //Assert

            Assert.Equal(nameof(DysacShortQuestionContentItemModel), result.GetType().BaseType.Name);
        }

        [Fact]
        public void MappingServiceWhenPassedDysacTraitReturnsQuestionSet()
        {
            //Arrange
            var mappingService = new MappingService(A.Fake<IMapper>());

            //Act
            var result = mappingService.Map(new DysacTraitContentModel(), new ApiGenericChild());

            //Assert
            Assert.Equal(nameof(DysacTraitContentModel), result.GetType().BaseType.Name);
        }

        [Fact]
        public void MappingServiceWhenPassedDysacSkillReturnsQuestionSet()
        {
            //Arrange
            var mappingService = new MappingService(A.Fake<IMapper>());

            //Act
            var result = mappingService.Map(new DysacSkillContentModel(), new ApiGenericChild());

            //Assert
            Assert.Equal(nameof(DysacSkillContentModel), result.GetType().BaseType.Name);
        }

        [Fact]
        public void MappingServiceWhenPassedJobCategoryReturnsQuestionSet()
        {
            //Arrange
            var mappingService = new MappingService(A.Fake<IMapper>());

            //Act
            var result = mappingService.Map(new JobCategoryContentItemModel(), new ApiGenericChild());

            //Assert
            Assert.Equal(nameof(JobCategoryContentItemModel), result.GetType().BaseType.Name);
        }

        [Fact]
        public void MappingServiceWhenPassedWrongThrowsException()
        {
            //Arrange
            var mappingService = new MappingService(A.Fake<IMapper>());

            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() => mappingService.Map(new NonImplementedClassForTesting(), new ApiGenericChild()));
        }
    }

    public class NonImplementedClassForTesting : IDysacContentModel
    {
        public Uri? Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<Guid>? AllContentItemIds => throw new NotImplementedException();

        public DateTime? LastCached { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid? ItemId => throw new NotImplementedException();

        public List<IDysacContentModel>? GetContentItems()
        {
            throw new NotImplementedException();
        }

        public void RemoveContentItem(Guid contentItemId)
        {
            throw new NotImplementedException();
        }
    }
}
