using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers.QuestionsAnswers;

namespace DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers
{
    public static class Support
    {
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

        public static void ScrollIntoViewJavaScript(IWebDriver driver, IWebElement elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].scrollIntoView();", locator);
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'nearest'})", elementLocator);
        }
    }
}