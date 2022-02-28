using DFC.App.DiscoverSkillsCareers.TestSuite;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Extensions
{
    public static class ScenarioContentExtension
    {
        #region contants
        private const string WebDriverKey = "webdriver";
        private const string EnvSettingsKey = "envsettings";
        #endregion

        public static void SetWebDriver(this SpecFlowContext context, IWebDriver webDriver)
        {
            Set(context, webDriver, WebDriverKey);
        }

        public static IWebDriver GetWebDriver(this SpecFlowContext context)
        {
            return Get<IWebDriver>(context, WebDriverKey);
        }

        public static void SetEnv(this SpecFlowContext context, EnvironmentSettings envSettings)
        {
            Set(context, envSettings, EnvSettingsKey);
        }

        public static EnvironmentSettings GetEnv(this SpecFlowContext context)
        {
            return Get<EnvironmentSettings>(context, EnvSettingsKey);
        }

        private static void Set<T>(SpecFlowContext context, T value, string key)
        {
            context.Set(value, key);
        }

        public static T Get<T>(SpecFlowContext context, string key)
        {
            return context.Get<T>(key);
        }

        public static void AddFeatureFailure(this SpecFlowContext context, string messageContext, string message)
        {
            Dictionary<string, string> failures;
            if (context.ContainsKey(Constants.featureFailure))
            {
                failures = (Dictionary<string, string>)context[Constants.featureFailure];
            }
            else 
            {
                failures = new Dictionary<string, string>();
            }
            failures.Add(messageContext, message);
            context[Constants.featureFailure] = failures;
        }

        public static Dictionary<string,string> GetFeatureFailure(this SpecFlowContext context)
        {
            return context.ContainsKey(Constants.featureFailure) ? (Dictionary<string, string>)context[Constants.featureFailure] : new Dictionary<string, string>();
        }
    }
}
