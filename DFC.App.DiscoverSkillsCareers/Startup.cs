using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Framework;
using DFC.App.DiscoverSkillsCareers.HostedServices;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Sessionstate;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Data.Models.PollyOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Logger.AppInsights.Contracts;
using DFC.Logger.AppInsights.Extensions;
using Dfc.Session;
using Dfc.Session.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
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
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
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

                MapRoute(endpoints, "assessment", RouteName.Prefix + "/assessment/{assessmentType}/{questionNumber}", "Assessment", "Index");
                MapRoute(endpoints, "assessment", RouteName.Prefix + "/reload", "Assessment", "Reload");
                MapRoute(endpoints, "filterQuestionsComplete", RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/complete", "FilterQuestions", "Complete");
                MapRoute(endpoints, "filterQuestions", RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}", "FilterQuestions", "Index");
                MapRoute(endpoints, "testLoadSession", RouteName.Prefix + "/loadsession", "Test", "LoadSession");
                MapRoute(endpoints, "root", RouteName.Prefix, "Home", "Index");
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
            services.AddApplicationInsightsTelemetry();

            services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
            services.AddScoped<ISerialiser, NewtonsoftSerialiser>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IResultsService, ResultsService>();
            services.AddScoped<IDataProcessor<GetQuestionResponse>, GetQuestionResponseDataProcessor>();
            services.AddScoped<IDataProcessor<GetAssessmentResponse>, GetAssessmentResponseDataProcessor>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionIdToCodeConverter, SessionIdToCodeConverter>();
            services.AddSingleton(Configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            services.AddTransient<ICacheReloadService, CacheReloadService>();
            services.AddTransient<IEventMessageService, EventMessageService>();

            var cosmosDbConnectionContent = Configuration.GetSection("Configuration:CosmosDbConnections:DysacContent").Get<CosmosDbConnection>();
            services.AddDocumentServices<DysacQuestionSetContentModel>(cosmosDbConnectionContent, env.IsDevelopment());
            services.AddDocumentServices<DysacTraitContentModel>(cosmosDbConnectionContent, env.IsDevelopment());
            services.AddDocumentServices<DysacSkillContentModel>(cosmosDbConnectionContent, env.IsDevelopment());
            services.AddDocumentServices<DysacFilteringQuestionContentModel>(cosmosDbConnectionContent, env.IsDevelopment());

            var cosmosDbConnectionAssessment = Configuration.GetSection("Configuration:CosmosDbConnections:DysacAssessment").Get<CosmosDbConnection>();
            services.AddDocumentServices<DysacAssessment>(cosmosDbConnectionAssessment, env.IsDevelopment());

            services.AddTransient<IDocumentServiceFactory, DocumentServiceFactory>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IMappingService, MappingService>();

            services.AddTransient<IContentProcessor, DysacQuestionSetContentProcessor>();
            services.AddTransient<IContentProcessor, DysacTraitContentProcessor>();
            services.AddTransient<IContentProcessor, DysacSkillContentProcessor>();

            services.AddTransient<IAssessmentCalculationService, AssessmentCalculationService>();

            var cosmosDbConnectionSessionState = Configuration.GetSection("Configuration:CosmosDbConnections:SessionState").Get<CosmosDbConnection>();
            services.AddSessionStateServices<DfcUserSession>(cosmosDbConnectionSessionState, env.IsDevelopment());

            services.AddDFCLogging(Configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddDFCLogging(this.Configuration["ApplicationInsights:InstrumentationKey"]);
            var policyRegistry = services.AddPolicyRegistry();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = Configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            AddPolicies(policyRegistry);

            services.AddPolicies(policyRegistry, "content", policyOptions);
            services.AddHostedServiceTelemetryWrapper();
            services.AddSubscriptionBackgroundService(Configuration);
            services.AddHostedService<CacheReloadBackgroundService>();

            services.AddApiServices(Configuration, policyRegistry);
        }

        private static void AddPolicies(IPolicyRegistry<string> policyRegistry)
        {
            var policyOptions = new PolicyOptions() { HttpRetry = new RetryPolicyOptions(), HttpCircuitBreaker = new CircuitBreakerPolicyOptions() };
            policyRegistry.Add(nameof(PolicyOptions.HttpRetry), HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(policyOptions.HttpRetry.Count, retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(nameof(PolicyOptions.HttpCircuitBreaker), HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking, policyOptions.HttpCircuitBreaker.DurationOfBreak));
        }

        private static void MapRoute(IEndpointRouteBuilder routeBuilder, string name, string pattern, string controller, string action)
        {
            routeBuilder.MapControllerRoute(name, pattern, new { controller, action });
        }
    }
}
