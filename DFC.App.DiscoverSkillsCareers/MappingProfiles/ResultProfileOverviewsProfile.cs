using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    public class ResultProfileOverviewsProfile : Profile
    {
        public ResultProfileOverviewsProfile()
        {
            CreateMap<GetResultsResponse, ResultsByCategoryModel>()
            .ForMember(d => d.JobsInCategory, s => s.MapFrom(a => a.JobCategories))
            .ForMember(d => d.AssessmentReference, s => s.Ignore())
            .ForMember(d => d.ExploreCareersUri, s => s.Ignore());

            CreateMap<JobCategoryResult, ResultsJobsInCategoryModel>()
            .ForMember(d => d.CategoryTitle, s => s.MapFrom(a => a.JobFamilyName))
            .ForMember(d => d.CategoryUrl, s => s.MapFrom(a => a.JobFamilyUrl))
            .ForMember(d => d.CategoryCode, s => s.MapFrom(a => a.JobFamilyCode))
            .ForMember(d => d.JobProfiles, s => s.MapFrom(a => a.JobProfilesOverviews))
            .ForMember(d => d.AnswerMoreQuetions, s => s.MapFrom(a => a.TotalQuestions))
            .ForMember(d => d.ShowThisCategory, s => s.MapFrom(a => a.ResultsShown))
            .ForMember(d => d.NumberOfSuitableRoles, s => s.Ignore());

            CreateMap<JobProfileResult, ResultJobProfileModel>()
            .ForMember(d => d.JobProfilesOverview, s => s.Ignore());

            CreateMap<JobProfileOverView, ResultJobProfileOverViewModel>();
        }
    }
}
