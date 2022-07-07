using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Extensions
{
    public static class WebDriverExtension
    {
        private static TimeSpan maxWaitTime = new TimeSpan(0, 0, 30);

        public static bool ClickButton(this IWebDriver driver, string buttonText)
        {
            if (ClickButtonByText(driver, buttonText)) return true;
            if (ClickButtonById(driver, buttonText)) return true;
            if (ClickButtonByLinkText(driver, buttonText)) return true;
            if (ClickButtonByCssSelector(driver, buttonText)) return true;
            if (ClickButtonByContainsText(driver, buttonText)) return true;

            throw new Exception("Unable to locate button");

        }

        public static IWebElement WaitUntilElementFound(this IWebDriver driver, By elementId)
        {
            var wait = new WebDriverWait(driver, maxWaitTime);
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(elementId));
        }

        public static IWebElement WaitElementToBeClickable(this IWebDriver driver, By elementLocator)
        {
            var wait = new WebDriverWait(driver, maxWaitTime);
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(elementLocator));
        }

        public static bool ClickButtonByText(this IWebDriver driver, string buttonText)
        {
            try
            {
                driver.FindElement(By.XPath("//button[text()='" + buttonText + "']")).Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool ClickButtonByCssSelector(this IWebDriver driver, string cssSelector)
        {
            try
            {
                driver.FindElement(By.CssSelector(cssSelector)).Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool ClickButtonByContainsText(this IWebDriver driver, string buttonText)
        {
            try
            {
                driver.FindElement(By.XPath("//*[text()[contains(.,'" + buttonText + "')]]")).Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ClickButtonById(this IWebDriver driver, string buttonId)
        {
            try
            {
                driver.FindElement(By.Id(buttonId)).Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ClickButtonByLinkText(this IWebDriver driver, string linkText)
        {
            try
            {
                driver.FindElement(By.LinkText(linkText)).Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SelectDropListItemById(this IWebDriver driver, string listId, string textValue)
        {
            try
            {
                var element = driver.FindElement(By.Id(listId));
                var selectElement = new SelectElement(element);
                selectElement.SelectByText(textValue);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SelectDropListItemByClass(this IWebDriver driver, string classId, string textValue)
        {
            try
            {
                var element = driver.FindElement(By.ClassName(classId));

                Actions builder = new Actions(driver);
                var mouseUp = builder.MoveToElement(element)
                                     .Click()
                                     .Build();
                mouseUp.Perform();

                element.SendKeys(textValue);
                element.SendKeys(Keys.Enter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SelectButtonGroupClass(this IWebDriver driver, string classId, int index)
        {
            // first index is 1
            try
            {
                var elements = driver.FindElements(By.ClassName(classId));

                if (elements.Count >= index)
                {
                    Actions builder = new Actions(driver);
                    var mouseUp = builder.MoveToElement(elements[index - 1])
                                         .Click()
                                         .Build();
                    mouseUp.Perform();

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ClickOnItem(this IWebDriver driver, string id)
        {
            try
            {
                var element = driver.FindElement(By.Id(id));
                Actions builder = new Actions(driver);
                var mouseUp = builder.MoveToElement(element)
                                     .Click()
                                     .Build();
                mouseUp.Perform();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to click on item: {id} - {e.Message}");
            }
            return true;
        }
    }
}
