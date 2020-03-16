using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using Dfc.Session;
using Dfc.Session.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Diagnostics.CodeAnalysis;

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
                    name: "assessment",
                    pattern: RouteName.Prefix + "/reload",
                    new { controller = "Assessment", action = "Reload" });

                endpoints.MapControllerRoute(
                    name: "filterQuestionsComplete",
                    pattern: RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/complete",
                    new { controller = "FilterQuestions", action = "Complete" });

                endpoints.MapControllerRoute(
                    name: "filterQuestions",
                    pattern: RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}",
                    new { controller = "FilterQuestions", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "jobProfileOverviews",
                    pattern: RouteName.Prefix + "/results/{jobCategoryName}",
                    new { controller = "Results", action = "JobProfileOverviews" });

                endpoints.MapControllerRoute(
                    name: "root",
                    pattern: RouteName.Prefix,
                    new { controller = "Home", action = "Index" });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var sessionServiceConfig = Configuration.GetSection("SessionConfig").Get<SessionConfig>();
            services.AddSessionServices(sessionServiceConfig);

            var notifyOptions = Configuration.GetSection("Notify").Get<NotifyOptions>();
            services.AddSingleton(notifyOptions);

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<ISerialiser, NewtonsoftSerialiser>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IResultsService, ResultsService>();
            services.AddScoped<IDataProcessor<GetQuestionResponse>, GetQuestionResponseDataProcessor>();
            services.AddScoped<IDataProcessor<GetAssessmentResponse>, GetAssessmentResponseDataProcessor>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionIdToCodeConverter, SessionIdToCodeConverter>();

            var dysacClientOptions = Configuration.GetSection("DysacClientOptions").Get<DysacClientOptions>();
            var policyRegistry = services.AddPolicyRegistry();
            AddPolicies(policyRegistry);

            services.AddHttpClient<IResultsApiService, ResultsApiService>(
                httpClient =>
                {
                    httpClient.BaseAddress = dysacClientOptions.ResultsApiBaseAddress;
                    httpClient.Timeout = dysacClientOptions.Timeout;
                    httpClient.DefaultRequestHeaders.Add(HeaderName.OcpApimSubscriptionKey, dysacClientOptions.OcpApimSubscriptionKey);
                }).AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpRetry))
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpCircuitBreaker));

            services.AddHttpClient<IAssessmentApiService, AssessmentApiService>(
                httpClient =>
                {
                    httpClient.BaseAddress = dysacClientOptions.AssessmentApiBaseAddress;
                    httpClient.Timeout = dysacClientOptions.Timeout;
                    httpClient.DefaultRequestHeaders.Add(HeaderName.OcpApimSubscriptionKey, dysacClientOptions.OcpApimSubscriptionKey);
                }).AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpRetry))
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpCircuitBreaker));

            var jobProfileOverViewClientOptions = Configuration.GetSection("JobProfileOverViewClientOptions").Get<JobProfileOverViewClientOptions>();

            services.AddHttpClient<IJpOverviewApiService, JpOverviewApiService>(httpClient =>
            {
                httpClient.Timeout = jobProfileOverViewClientOptions.Timeout;
                httpClient.BaseAddress = jobProfileOverViewClientOptions.BaseAddress;
            }).AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpRetry))
            .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.HttpCircuitBreaker));
        }

        private static void AddPolicies(IPolicyRegistry<string> policyRegistry)
        {
            var policyOptions = new PolicyOptions() { HttpRetry = new RetryPolicyOptions(),  HttpCircuitBreaker = new CircuitBreakerPolicyOptions() };
            policyRegistry.Add(nameof(PolicyOptions.HttpRetry), HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(policyOptions.HttpRetry.Count, retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(nameof(PolicyOptions.HttpCircuitBreaker), HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking, policyOptions.HttpCircuitBreaker.DurationOfBreak));
        }
    }
}
