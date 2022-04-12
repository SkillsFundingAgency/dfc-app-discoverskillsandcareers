using System;
using System.IO;
using System.Reflection;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
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
        //Extent reports
        private static string filePath = Directory.GetParent(@"../../../").FullName + Path.DirectorySeparatorChar + "Result" + "\\";
        private static AventStack.ExtentReports.ExtentReports extentReports;
        private static ExtentHtmlReporter extentHtmlReporter;
        private static ExtentTest feature;
        private static ExtentTest scenario;

        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        public SetupContext(FeatureContext fContext, ScenarioContext sContext)
        {
            _featureContext = fContext;
            _scenarioContext = sContext;
        }

        //Extent reports
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            extentHtmlReporter = new ExtentHtmlReporter(filePath);
            extentHtmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            extentReports = new AventStack.ExtentReports.ExtentReports();
            extentReports.AttachReporter(extentHtmlReporter);
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext context)
        {
            if (context != null)
            {
                feature = extentReports.CreateTest<Feature>(context.FeatureInfo.Title, context.FeatureInfo.Description);
            }
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null)
            {
                throw new ArgumentNullException(nameof(scenarioContext));
            }

            ScenarioBlock scenarioBlock = scenarioContext.CurrentScenarioBlock;

            switch (scenarioBlock)
            {
                case ScenarioBlock.Given:
                    ReportExecution(scenarioContext);
                    break;
                case ScenarioBlock.When:
                    ReportExecution(scenarioContext);
                    break;
                case ScenarioBlock.Then:
                    ReportExecution(scenarioContext);
                    break;
                default:
                    ReportExecution(scenarioContext);
                    break;
            }
        }

        public void ReportExecution(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null)
            {
                throw new ArgumentNullException(nameof(scenarioContext));
            }

            if (scenarioContext.TestError != null)
            {
                //screenShot.TakeScreenShot(scenarioContext.GetWebDriver(), filePath);

                scenario.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message);
            }
            else
            {
                scenario.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text).Pass(string.Empty);
            }
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            extentReports.Flush();
        }

        [BeforeScenario]
        public static void BeforeScenarioStart(ScenarioContext scenarioContext)
        {
            if (scenarioContext != null)
            {
                scenario = feature.CreateNode<Scenario>(scenarioContext.ScenarioInfo.Title, scenarioContext.ScenarioInfo.Description);
            }
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
        
    }
}
