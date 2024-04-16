using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Framework;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Sessionstate;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.PollyOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Logger.AppInsights.Contracts;
using DFC.Logger.AppInsights.Extensions;
using Dfc.Session;
using Dfc.Session.Models;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Interfaces;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace DFC.App.DiscoverSkillsCareers
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string CosmosDbConnectionAssessmentAppSettings = "Configuration:CosmosDbConnections:DysacAssessment";
        private const string CosmosDbConnectionSessionStateAppSettings = "Configuration:CosmosDbConnections:SessionState";
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";
        private const string WorkerThreadsConfigAppSettings = "ThreadSettings:WorkerThreads";
        private const string IocpThreadsConfigAppSettings = "ThreadSettings:IocpThreads";
        private readonly ILogger<Startup> logger;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.env = env;
            this.logger = logger;
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

                MapRoute(endpoints, "results", RouteName.Prefix + "/results/{countToShow}", "Results", "Index");
                MapRoute(endpoints, "assessmentNew", RouteName.Prefix + "/assessment/{assessmentType}/new", "Assessment", "New");
                MapRoute(endpoints, "assessment", RouteName.Prefix + "/assessment/{assessmentType}/{questionNumber}", "Assessment", "Index");
                MapRoute(endpoints, "assessmentReload", RouteName.Prefix + "/reload", "Assessment", "Reload");
                MapRoute(endpoints, "filterQuestionsComplete", RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/complete", "FilterQuestions", "Complete");
                MapRoute(endpoints, "filterQuestions", RouteName.Prefix + "/{assessmentType}/filterquestions/{jobCategoryName}/{questionNumber}", "FilterQuestions", "Index");
                MapRoute(endpoints, "testLoadSession", RouteName.Prefix + "/loadsession", "Test", "LoadSession");
                MapRoute(endpoints, "root", RouteName.Prefix, "Home", "Index");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureMinimumThreads();

            services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>(); });
            services.AddHttpClient();
            services.AddSingleton<IGraphQLClient>(s =>
            {
                var option = new GraphQLHttpClientOptions()
                {
                    EndPoint = new Uri(Configuration[ConfigKeys.GraphApiUrl]),
                    HttpMessageHandler = new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                };
                var client = new GraphQLHttpClient(option, new NewtonsoftJsonSerializer());
                return client;
            });

            services.AddSingleton<ISharedContentRedisInterfaceStrategy<SharedHtml>, SharedHtmlQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PersonalityQuestionSet>, DysacQuestionSetQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<JobProfilesResponse>, JobProfileOverviewQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PersonalityFilteringQuestionResponse>, DysacFilteringQuestionQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<JobProfileCategoriesResponseDysac>, JobCategoryQueryStrategyDysac>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PersonalityTraitResponse>, TraitsQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyFactory, SharedContentRedisStrategyFactory>();
            services.AddScoped<ISharedContentRedisInterface, SharedContentRedis>();

            var sessionServiceConfig = Configuration.GetSection("SessionConfig").Get<SessionConfig>();
            services.AddSessionServices(sessionServiceConfig);

            var notifyOptions = Configuration.GetSection("Notify").Get<NotifyOptions>();
            services.AddSingleton(notifyOptions);

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(DysacProfile)), Assembly.GetAssembly(typeof(DefaultProfile)));

            services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
            services.AddScoped<ISerialiser, NewtonsoftSerialiser>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IResultsService, ResultsService>();
            services.AddScoped<IDataProcessor<GetQuestionResponse>, GetQuestionResponseDataProcessor>();
            services.AddScoped<IDataProcessor<GetAssessmentResponse>, GetAssessmentResponseDataProcessor>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionIdToCodeConverter, SessionIdToCodeConverter>();
            services.AddSingleton(Configuration.GetSection(nameof(DysacOptions)).Get<DysacOptions>() ?? new DysacOptions());
            services.AddTransient<INotificationService, NotificationService>();
            services.AddSingleton<INotificationClient>(new NotificationClient(Configuration["Notify:ApiKey"]));

            services.AddTransient<ISharedContentRedisInterface, SharedContentRedis>();
            services.AddTransient<ISharedContentRedisInterfaceStrategyFactory, SharedContentRedisStrategyFactory>();
            services.AddRazorTemplating();

            services.AddTransient<IMappingService, MappingService>();

            services.AddTransient<CosmosDbAppInsightsRequestHandler>();

            services.AddSingleton<IDocumentStore, CosmosDbService>(serviceProvider =>
            {
                var cosmosDbConnectionAssessment = Configuration.GetSection(CosmosDbConnectionAssessmentAppSettings).Get<CosmosDbConnection>();
                var connectionStringAssessment = $"AccountEndpoint={cosmosDbConnectionAssessment.EndpointUrl};AccountKey={cosmosDbConnectionAssessment.AccessKey};";

                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });
                var logger = serviceProvider.GetRequiredService<ILogger<CosmosDbService>>();
                var assessmentRequestHandler = serviceProvider.GetRequiredService<CosmosDbAppInsightsRequestHandler>();

                return new CosmosDbService(
                connectionStringAssessment,
                cosmosDbConnectionAssessment.DatabaseId!,
                cosmosDbConnectionAssessment.CollectionId!,
                logger,
                assessmentRequestHandler);
            });

            services.AddTransient<IAssessmentCalculationService, AssessmentCalculationService>();

            var cosmosDbConnectionSessionState = Configuration.GetSection(CosmosDbConnectionSessionStateAppSettings).Get<CosmosDbConnection>();
            services.AddSessionStateServices<DfcUserSession>(cosmosDbConnectionSessionState, env.IsDevelopment());

            services.AddTransient<IApiCacheService, ApiCacheService>();

            services.AddDFCLogging(Configuration["ApplicationInsights:InstrumentationKey"]);
            var policyRegistry = services.AddPolicyRegistry();

            const string appSettingsPolicies = "Policies";
            var policyOptions = Configuration.GetSection(appSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            AddPolicies(policyRegistry);

            services.AddPolicies(policyRegistry, "content", policyOptions);
            services.AddHostedServiceTelemetryWrapper();

            services.AddApiServices(Configuration, policyRegistry);
            services.AddLinkDetailsConverter(new CustomLinkDetailConverter());

        }

        private static void AddPolicies(IPolicyRegistry<string> policyRegistry)
        {
            var policyOptions = new PolicyOptions { HttpRetry = new RetryPolicyOptions(), HttpCircuitBreaker = new CircuitBreakerPolicyOptions() };
            policyRegistry.Add(nameof(PolicyOptions.HttpRetry), HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(policyOptions.HttpRetry.Count, retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(nameof(PolicyOptions.HttpCircuitBreaker), HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking, policyOptions.HttpCircuitBreaker.DurationOfBreak));
        }

        private static void MapRoute(IEndpointRouteBuilder routeBuilder, string name, string pattern, string controller, string action)
        {
            routeBuilder.MapControllerRoute(name, pattern, new { controller, action });
        }

        private void ConfigureMinimumThreads()
        {
            var workerThreads = Convert.ToInt32(Configuration[WorkerThreadsConfigAppSettings]);

            var iocpThreads = Convert.ToInt32(Configuration[IocpThreadsConfigAppSettings]);

            if (ThreadPool.SetMinThreads(workerThreads, iocpThreads))
            {
                logger.LogInformation(
                    "ConfigureMinimumThreads: Minimum configuration value set. IOCP = {0} and WORKER threads = {1}",
                    iocpThreads,
                    workerThreads);
            }
            else
            {
                logger.LogWarning("ConfigureMinimumThreads: The minimum number of threads was not changed");
            }
        }
    }
}
