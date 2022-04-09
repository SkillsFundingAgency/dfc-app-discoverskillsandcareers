using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers
{
    public static class Support
    {
        private static bool _hasRun;

        public static string[] GetAllText(IWebDriver driver, By locator)
        {
            IList<IWebElement> all = driver.FindElements(locator) ;

            String[] allText = new String[all.Count];

            int i = 0;
            foreach (IWebElement element in all)
            {
                allText[i++] = element.Text;
            }

            return allText;
        }

        public static void RunOnce(Action method)
        {
            if (!_hasRun)
            {
                method();
            }
            _hasRun = true;
        }
    }
}