using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Extensions
{
    public static class WebElementExtension
    {
        public static string GetElementText(this IWebElement element)
        {
            return element.Text.Trim();
        }
    }
}
