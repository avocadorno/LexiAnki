using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Contracts.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace HanziiAnki.Core.Services;

class SeleniumScappingService : IWebScrappingService
{
    private IWebDriver _driver;
    private IWebDriver WebDriver
    {
        get
        {
            if (_driver == null || IsBrowserClosed())
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--lang=vi");
                _driver = new ChromeDriver(options);
            }
            return _driver;
        }
    }

    private bool IsBrowserClosed()
    {
        try
        {
            var _ = _driver.Url;
            return false;
        }
        catch (NoSuchElementException)
        {
            return true;
        }
    }

    public string ScrapeWebsite(string url, string waitForElementCssSelector)
    {
        try
        {
            WebDriver.Navigate().GoToUrl(url);
            if (!String.IsNullOrEmpty(waitForElementCssSelector))
            {
                try
                {
                    var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
                    wait.Until(d =>
                    {
                        try
                        {
                            var element = d.FindElement(By.CssSelector(waitForElementCssSelector));
                            return true;
                        }
                        catch (NoSuchElementException)
                        {
                            return false;
                        }
                    });
                    return WebDriver.FindElement(By.CssSelector(waitForElementCssSelector)).GetAttribute("outerHTML");
                }
                catch (WebDriverTimeoutException)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
