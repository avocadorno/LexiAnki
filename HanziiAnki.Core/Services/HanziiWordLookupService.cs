using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Models;
using HtmlAgilityPack;

namespace HanziiAnki.Core.Services;

public class HanziiWordLookupService : IWordLookUpService
{
    private HtmlDocument _htmlDocument;
    private IWebScrappingService _webScrappingService;
    public HanziiWordLookupService()
    {
        _htmlDocument = new HtmlDocument();
        _webScrappingService = new SeleniumScappingService();
    }
    private string GetQueryURL(string keyword) => $"https://hanzii.net/search/word/{keyword}?hl=vi";

    public ChineseDeck GetWordDefinition(string keyword)
    {
        var deck = new ChineseDeck();
        _htmlDocument.LoadHtml(_webScrappingService.ScrapeWebsite(GetQueryURL(keyword), ".content-result"));
        return deck;
    }
}
