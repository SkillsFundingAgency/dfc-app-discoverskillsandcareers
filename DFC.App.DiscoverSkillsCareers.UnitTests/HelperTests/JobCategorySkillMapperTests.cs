using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Helpers
{
    public class JobCategorySkillMapperTests
    {
        private readonly IEnumerable<JobProfileContentItemModel> JobProfilesWithCommonSkills;
        private readonly IEnumerable<JobProfileContentItemModel> JobProfilesWithNoCommonSkills;

        public JobCategorySkillMapperTests()
        {
            JobProfilesWithCommonSkills = new List<JobProfileContentItemModel>
            {
                new JobProfileContentItemModel
                {
                    Title = "Job Profile 1",
                    Skills = new List<DysacSkillContentItemModel>
                    {
                        new DysacSkillContentItemModel { Ordinal = 0, ONetRank = 3.44m, Title = "SKILL1" },
                        new DysacSkillContentItemModel { Ordinal = 1, ONetRank = 1.75m, Title = "SKILL2" },
                        new DysacSkillContentItemModel { Ordinal = 2, ONetRank = 5.24m, Title = "SKILL3" },
                        new DysacSkillContentItemModel { Ordinal = 3, ONetRank = 0.14m, Title = "SKILL4" },
                        new DysacSkillContentItemModel { Ordinal = 4, ONetRank = 2.36m, Title = "SKILL5" },
                        new DysacSkillContentItemModel { Ordinal = 5, ONetRank = 2.84m, Title = "SKILL6" },
                    },
                },
                new JobProfileContentItemModel
                {
                    Title = "Job Profile 2",
                    Skills = new List<DysacSkillContentItemModel>
                    {
                        new DysacSkillContentItemModel { Ordinal = 1, ONetRank = 4.75m, Title = "SKILL2" },
                        new DysacSkillContentItemModel { Ordinal = 2, ONetRank = 1.24m, Title = "SKILL3" },
                        new DysacSkillContentItemModel { Ordinal = 3, ONetRank = 3.14m, Title = "SKILL4" },
                        new DysacSkillContentItemModel { Ordinal = 4, ONetRank = 2.10m, Title = "SKILL5" },
                        new DysacSkillContentItemModel { Ordinal = 5, ONetRank = 2.32m, Title = "SKILL7" },
                    },
                },
            };

            JobProfilesWithNoCommonSkills = new List<JobProfileContentItemModel>
            {
                new JobProfileContentItemModel
                {
                    Title = "Job Profile 1",
                    Skills = new List<DysacSkillContentItemModel>
                    {
                        new DysacSkillContentItemModel { Ordinal = 0, ONetRank = 3.44m, Title = "SKILLA" },
                        new DysacSkillContentItemModel { Ordinal = 1, ONetRank = 1.75m, Title = "SKILLB" },
                        new DysacSkillContentItemModel { Ordinal = 2, ONetRank = 5.24m, Title = "SKILLC" },
                        new DysacSkillContentItemModel { Ordinal = 3, ONetRank = 0.14m, Title = "SKILLD" },
                        new DysacSkillContentItemModel { Ordinal = 4, ONetRank = 2.36m, Title = "SKILLE" },
                        new DysacSkillContentItemModel { Ordinal = 5, ONetRank = 2.84m, Title = "SKILLF" },
                    },
                },
                new JobProfileContentItemModel
                {
                    Title = "Job Profile 2",
                    Skills = new List<DysacSkillContentItemModel>
                    {
                        new DysacSkillContentItemModel { Ordinal = 1, ONetRank = 4.75m, Title = "SKILL2" },
                        new DysacSkillContentItemModel { Ordinal = 2, ONetRank = 1.24m, Title = "SKILL3" },
                        new DysacSkillContentItemModel { Ordinal = 3, ONetRank = 3.14m, Title = "SKILL4" },
                        new DysacSkillContentItemModel { Ordinal = 4, ONetRank = 2.10m, Title = "SKILL5" },
                        new DysacSkillContentItemModel { Ordinal = 5, ONetRank = 2.32m, Title = "SKILL7" },
                    },
                },
            };
        }

        [Fact]
        public void JobCategorySkillMappingHelperCalculateCommonSkillsToRemoveByPercentageReturns4Skills()
        {
            // Arrange
            // Act
            var result = JobCategorySkillMappingHelper.CalculateCommonSkillsToRemoveByPercentage(JobProfilesWithCommonSkills.ToList(), 0.75);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Contains("SKILL2", result);
            Assert.Contains("SKILL3", result);
            Assert.Contains("SKILL4", result);
            Assert.Contains("SKILL5", result);
        }

        [Fact]
        public void JobCategorySkillMappingHelperCalculateCommonSkillsToRemoveByPercentageNoCommonReturnsNoSkills()
        {
            // Arrange
            // Act
            var result = JobCategorySkillMappingHelper.CalculateCommonSkillsToRemoveByPercentage(JobProfilesWithNoCommonSkills.ToList(), 0.75);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void JobCategorySkillMappingHelperGetSkillAttributesReturnsAttributes()
        {
            // Arrange
            // Act
            var result = JobCategorySkillMappingHelper.GetSkillAttributes(
                JobProfilesWithCommonSkills,
                new HashSet<string> { "SKILL7", "SKILL1" },
                0.75,
                JobProfilesWithCommonSkills.SelectMany(x => x.Skills).Select(x => x.Title).ToList());

            // Assert
            Assert.Equal(5, result.Count());
        }
    }
}
