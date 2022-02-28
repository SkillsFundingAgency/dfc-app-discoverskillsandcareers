using System;
using System.IO;
using System.Reflection;

using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;

using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Hooks
{
    [Binding]
    public sealed class SetupContext
    {
        private const string mask = "########################################################################################################################################";
        readonly FeatureContext _featureContext;
        readonly ScenarioContext _scenarioContext;
        private int _WebdriverTimeoutSeconds = 0; // 0 means use default value
        private const int _WebdriverExtendedTimeout = 1200;

        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        public SetupContext(FeatureContext fContext, ScenarioContext sContext)
        {
            _featureContext = fContext;
            _scenarioContext = sContext;
        }

        [BeforeScenario("longrunning", Order = 10)]
        public void SetLongRunningTimeout()
        {
            _WebdriverTimeoutSeconds = _WebdriverExtendedTimeout;

        }

        [BeforeScenario("webtest", Order = 20)]
        public void IntialiseWebDriver()
        {
            _scenarioContext.SetWebDriver(WebDriverContainer.Instance.GetWebDriver(FindDriverService(), _WebdriverTimeoutSeconds));
            _scenarioContext.GetWebDriver().Manage().Window.Maximize();
        }

        private string FindDriverService()
        {
            string driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string driverName = "chromedriver.exe";

            FileInfo[] file = Directory.GetParent(driverPath).GetFiles(driverName, SearchOption.AllDirectories);

            var driverLocation = file.Length != 0 ? file[0].DirectoryName : driverPath;

            Console.WriteLine($"Driver Service should be available under: {driverLocation}");

            return driverLocation;
        }

        [BeforeScenario(Order = 1)]
        public void IntialiseEnvironementVariables()
        {
            string value;
            string name;
            _scenarioContext.SetEnv(new EnvironmentSettings());

            if (_featureContext.TryGetValue(Constants.featureFailAll, out bool featureFailAll) && featureFailAll)
            {
                throw new Exception("Feature run aborted due to earlier failure");
            }

            PropertyInfo[] properties = typeof(EnvironmentSettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                name = property.Name;
                value = string.Empty;
                try
                {
                    if (property.PropertyType == typeof(Boolean))
                    {
                        value = ((bool)property.GetValue(_scenarioContext.GetEnv())).ToString();
                    }
                    else
                    {
                        value = (string)property.GetValue(_scenarioContext.GetEnv());
                        if (name.ToLower().Contains("key") || name.ToLower().Contains("password") || name.Contains("pwd") || name.Contains("secret"))
                        {
                            value = mask.Substring(0, value.Length);
                        }
                    }
                }
                catch { /* */ }
                Console.WriteLine($"Env: {property.Name} = {value}");
            }
        }

        //[BeforeScenario(Order = 5)]
        //public void CheckConnections()
        //{
        //    // check SQL connection
        //    if (_scenarioContext.GetEnv().SqlServerChecksEnabled)
        //    {
        //        var conn = _scenarioContext.GetSQLConnection();
        //        if (!conn.CheckPermissions(new[] { "SELECT", "DELETE" }))
        //        {
        //            _featureContext[Constants.featureFailAll] = true;
        //            throw new Exception("Unable to verify permission on SQL connection");
        //        }
        //    }
        //    // check Graph connections
        //    try
        //    {
        //        _scenarioContext.GetGraphConnection(Constants.publish);
        //    }
        //    catch (Exception e)
        //    {
        //        _featureContext[Constants.featureFailAll] = true;
        //        Console.WriteLine("Unable to verify connection to publish graph");
        //        throw e;
        //    }

        //    try
        //    {
        //        _scenarioContext.GetGraphConnection(Constants.preview);
        //    }
        //    catch (Exception e)
        //    {
        //        _featureContext[Constants.featureFailAll] = true;
        //        Console.WriteLine("Unable to verify connection to preview graph");
        //        throw e;
        //    }

        //    if (_scenarioContext.GetEnv().Neo4JUrl1.Length > 0)
        //    {
        //        try
        //        {
        //            _scenarioContext.GetGraphConnection(Constants.publish, 1);
        //        }
        //        catch (Exception e)
        //        {
        //            _featureContext[Constants.featureFailAll] = true;
        //            Console.WriteLine("Unable to verify connection to publish1 graph");
        //            throw e;
        //        }
        //    }

        //    if (_scenarioContext.GetEnv().Neo4JUrlDraft1.Length > 0)
        //    {
        //        try
        //        {
        //            _scenarioContext.GetGraphConnection(Constants.preview, 1);
        //        }
        //        catch (Exception e)
        //        {
        //            _featureContext[Constants.featureFailAll] = true;
        //            Console.WriteLine("Unable to verify connection to preview1 graph");
        //            throw e;
        //        }
        //    }
        //}
    }
}
