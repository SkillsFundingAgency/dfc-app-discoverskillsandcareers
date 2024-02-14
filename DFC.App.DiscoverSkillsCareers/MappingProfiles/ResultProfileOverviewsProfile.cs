using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.Azure.Documents.SystemFunctions;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    public class ResultProfileOverviewsProfile : Profile
    {
        public ResultProfileOverviewsProfile()
        {
            CreateMap<GetResultsResponse, ResultsByCategoryModel>()
            .ForMember(d => d.JobsInCategory, s => s.MapFrom(a => a.JobCategories))
            .ForMember(d => d.AssessmentReference, s => s.Ignore());

            CreateMap<JobCategoryResult, ResultsJobsInCategoryModel>()
            .ForMember(d => d.CategoryTitle, s => s.MapFrom(a => a.JobFamilyName))
            .ForMember(d => d.CategoryUrl, s => s.MapFrom(a => a.JobFamilyNameUrl))
            .ForMember(d => d.CategoryCode, s => s.MapFrom(a => a.JobFamilyCode))
            .ForMember(d => d.JobProfiles, s => s.MapFrom(a => a.JobProfilesOverviews))
            .ForMember(d => d.AnswerMoreQuestions, s => s.MapFrom(a => a.TotalQuestions))
            .ForMember(d => d.ShowThisCategory, s => s.MapFrom(a => a.ResultsShown))
            .ForMember(d => d.NumberOfSuitableRoles, s => s.Ignore());

            CreateMap<JobProfileResult, ResultJobProfileModel>()
            .ForMember(d => d.JobProfilesOverview, s => s.Ignore());

            CreateMap<JobProfileOverviewResponse, JobProfileViewModel>()
            .ForMember(d => d.DisplayText, s => s.MapFrom(a => a.JobProfile[0].DisplayText))
            .ForMember(d => d.Overview, s => s.MapFrom(a => a.JobProfile[0].Overview))
            .ForMember(d => d.UrlName, s => s.MapFrom(a => a.JobProfile[0].PageLocation.UrlName))
            .ForMember(d => d.WorkingPattern, s => s.MapFrom(a => a.JobProfile[0].WorkingPattern.ContentItems[0].DisplayText))
            .ForMember(d => d.WorkingPatternDetails, s => s.MapFrom(a => a.JobProfile[0].WorkingPatternDetails.ContentItems[0].DisplayText))
            .ForMember(d => d.WorkingHoursDetails, s => s.MapFrom(a => a.JobProfile[0].WorkingHoursDetails.ContentItems[0].DisplayText))
            .ForMember(d => d.MinimumHours, s => s.MapFrom(a => a.JobProfile[0].Minimumhours))
            .ForMember(d => d.MaximumHours, s => s.MapFrom(a => a.JobProfile[0].Maximumhours))
            .ForMember(d => d.SalaryStarterPerYear, s => s.MapFrom(a => a.JobProfile[0].Salarystarterperyear))
            .ForMember(d => d.SalaryExperiencedPerYear, s => s.MapFrom(a => a.JobProfile[0].Salaryexperiencedperyear))
            .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<JobProfileViewModel, ResultJobProfileOverViewModel>()
            .ForMember(d => d.Cname, s => s.MapFrom(a => a.DisplayText))
            .ForMember(d => d.OverViewHTML, s => s.MapFrom(a => a.Overview))
            .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<JobProfileOverView, ResultJobProfileOverViewModel>();

            CreateMap<GetResultsResponse, ResultsHeroBannerViewModel>()
            .ForMember(d => d.NumberOfCategories, s => s.MapFrom(a => a.JobCategories.Count()))
            .ForMember(d => d.IsCategoryBanner, s => s.Ignore());
        }
    }
}
