using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;


namespace DFC.App.DiscoverSkillsCareers.TestSuite.Hooks
{
    public sealed class WebDriverContainer
    {
        private ChromeDriver driver = null;
        private static readonly Lazy<WebDriverContainer> lazy =
            new Lazy<WebDriverContainer>(() => new WebDriverContainer());

        public static WebDriverContainer Instance { get { return lazy.Value; } }

        public ChromeDriver GetWebDriver(string path, int timeoutPeriod = 0)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            if (driver == null)
            {
                if (timeoutPeriod > 0)
                {
                    driver = new ChromeDriver(path, options, TimeSpan.FromSeconds(timeoutPeriod));
                }
                else
                {
                    driver = new ChromeDriver(path, options);
                }
            }
            return driver;
        }

        public void CloseDriver()
        {
            //if (driver != null)
            //{
            //    driver.Close();
            //    driver.Quit();
            //    driver = null;
            //}
        }
    }
}
