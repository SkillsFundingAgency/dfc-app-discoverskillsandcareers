using AutoMapper;
using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using DFC.App.DiscoverSkillsCareers.Services.Persistance;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using DFC.App.DiscoverSkillsCareers.Services.SessionIdToCodeConverters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace DFC.App.DiscoverSkillsCareers
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: RouteName.Prefix + "/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "assessment",
                    pattern: RouteName.Prefix + "/assessment/{assessmentType}/{questionNumber}",
                    new { controller = "Assessment", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "filterQuestionsComplete",
                    pattern: RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/complete",
                    new { controller = "FilterQuestions", action = "Complete" });

                endpoints.MapControllerRoute(
                    name: "filterQuestions",
                    pattern: RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}",
                    new { controller = "FilterQuestions", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "results",
                    pattern: RouteName.Prefix + "/results/{jobCategoryName}",
                    new { controller = "Results", action = "Filter" });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var sessionConfig = Configuration.GetSection("SessionConfig").Get<SessionConfig>();
            services.AddSessionServices(sessionConfig);

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<ISerialiser, NewtonsoftSerialiser>();
            services.AddScoped<IPersistanceService, CookiePersistanceService>();
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IDataProcessor<GetQuestionResponse>, GetQuestionResponseDataProcessor>();
            services.AddScoped<IDataProcessor<GetAssessmentResponse>, GetAssessmentResponseDataProcessor>();
            services.AddScoped<ISessionIdToCodeConverter, SessionIdToCodeConverter>();

            var dysacClientOptions = Configuration.GetSection("DysacClientOptions").Get<DysacClientOptions>();

            services.AddHttpClient<IAssessmentApiService, AssessmentApiService>(httpClient =>
            {
                ConfigureHttpClient(httpClient, dysacClientOptions.AssessmentApiBaseAddress, dysacClientOptions.OcpApimSubscriptionKey);
            });

            services.AddHttpClient<IResultsApiService, ResultsApiService>(httpClient =>
            {
                ConfigureHttpClient(httpClient, dysacClientOptions.ResultsApiBaseAddress, dysacClientOptions.OcpApimSubscriptionKey);
            });
        }

        private static void ConfigureHttpClient(HttpClient httpClient, Uri baseAddress, string ocpApimSubscriptionKey)
        {
            httpClient.DefaultRequestHeaders.Add(HeaderName.OcpApimSubscriptionKey, ocpApimSubscriptionKey);
            httpClient.BaseAddress = baseAddress;
        }
    }
}
