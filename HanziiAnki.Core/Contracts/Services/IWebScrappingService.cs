using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanziiAnki.Core.Contracts.Services;

interface IWebScrappingService
{
    public string ScrapeWebsite(string ur, string waitForElementCssSelector);
}
