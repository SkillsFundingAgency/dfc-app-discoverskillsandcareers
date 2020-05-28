﻿using DFC.App.DiscoverSkillsCareers.ClientHandlers;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Mime;

namespace DFC.App.DiscoverSkillsCareers.Extensions
{

    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolicies(
           this IServiceCollection services,
           IPolicyRegistry<string> policyRegistry,
           string keyPrefix,
           PolicyOptions policyOptions)
        {
            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpRetry)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpCircuitBreaker)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
                    this IServiceCollection services,
                    IConfiguration configuration,
                    string configurationSectionName,
                    string retryPolicyName,
                    string circuitBreakerPolicyName)
                    where TClient : class
                    where TImplementation : class, TClient
                    where TClientOptions : DysacClientOptions, new() =>
                    services
                        .Configure<TClientOptions>(configuration?.GetSection(configurationSectionName))
                        .AddHttpClient<TClient, TImplementation>()
                        .ConfigureHttpClient((sp, options) =>
                        {
                            var httpClientOptions = sp
                                .GetRequiredService<IOptions<TClientOptions>>()
                                .Value;
                            var dysacClientOptions = configuration?.GetSection("DysacClientOptions").Get<DysacClientOptions>();
                            options.BaseAddress = dysacClientOptions?.AssessmentApiBaseAddress;
                            options.Timeout = httpClientOptions.Timeout;
                            options.DefaultRequestHeaders.Clear();
                            options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
                        })
                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                        {
                            AllowAutoRedirect = false,
                        })
                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
                        .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                        .Services;
    }
}
