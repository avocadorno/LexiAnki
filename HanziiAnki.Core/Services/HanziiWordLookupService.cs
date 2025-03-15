using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace HanziiAnki.Core.Services;

public class HanziiWordLookupService : IWordLookUpService
{
    private readonly HtmlDocument _viHtmlDocument;
    private readonly HtmlDocument _enHtmlDocument;
    private readonly IWebScrappingService _webScrappingService;
    private const string ElementToWaitFor = ".detail-word";
    public HanziiWordLookupService()
    {
        _viHtmlDocument = new HtmlDocument();
        _enHtmlDocument = new HtmlDocument();
        _webScrappingService = new SeleniumScappingService();
    }
    private string GetQueryURL(string keyword, bool en) => $"https://hanzii.net/search/word/{keyword}?hl={(en ? "en" : "vi")}";

    public async Task<Card> GetWordDefinition(string keyword)
    {
        var card = new Card();
        _enHtmlDocument.LoadHtml(_webScrappingService.ScrapeWebsite(GetQueryURL(keyword, true), ElementToWaitFor));
        _viHtmlDocument.LoadHtml(_webScrappingService.ScrapeWebsite(GetQueryURL(keyword, false), ElementToWaitFor));

        var simplifiedTask = Task.Run(() => card.Simplfied = GetSimplified());
        var traditionalTask = Task.Run(() => card.Traditional = GetTraditional());
        var audioMaleURLTask = Task.Run(() => card.AudioMaleURL = GetAudioMaleURL());
        var audioFemaleURLTask = Task.Run(() => card.AudioFemaleURL = GetAudioFemaleURL());
        var pinyinTask = Task.Run(() => card.Pinyin = GetPinyin());
        var zhuyinTask = Task.Run(() => card.Zhuyin = GetZhuyin());
        var sinoVietnameseTask = Task.Run(() => card.SinoVietnamese = GetSinoVietnamese());
        var levelsTask = Task.Run(() => card.Levels = GetLevels());
        var radicalTask = Task.Run(() => card.Radical = GetRadical());
        var classifierTask = Task.Run(() => card.Classifier = GetClassifier());
        var enDefinitionsTask = Task.Run(() => card.EnDefinitions = GetDefinitions(true));
        var viDefinitionsTask = Task.Run(() => card.ViDefinitions = GetDefinitions(false));

        await Task.WhenAll(
            simplifiedTask,
            traditionalTask,
            audioMaleURLTask,
            audioFemaleURLTask,
            pinyinTask,
            zhuyinTask,
            sinoVietnameseTask,
            levelsTask,
            radicalTask,
            classifierTask,
            enDefinitionsTask,
            viDefinitionsTask);

        return card;
    }

    private string GetSimplified()
    {
        var characterNodes = _viHtmlDocument.QuerySelectorAll(".query-search .simple-tradition-wrap > .cl-item");
        return characterNodes.Any() ? String.Join("", characterNodes.Select(node => node.InnerText)) : String.Empty;
    }

    private string GetTraditional()
    {
        var characterNodes = _viHtmlDocument.QuerySelectorAll(".query-search .simple-tradition-wrap > .wrap-convert > .cl-item");
        return characterNodes.Any() ? String.Join("", characterNodes.Select(node => node.InnerText)) : GetSimplified();
    }

    private string getPhonetics(bool isPinyin)
    {
        var phoneticNodes = _viHtmlDocument.QuerySelectorAll(".detail-word .txt-word-multi .txt-pinyin");
        var index = isPinyin ? 0 : 1;
        if (phoneticNodes.Count > index + 1)
        {
            var phoneticText = phoneticNodes[index].InnerText;
            return phoneticText.Length > 2 ? phoneticText.Substring(1, phoneticText.Length - 2) : string.Empty;
        }
        return String.Empty;
    }

    private string GetPinyin()
    {
        return getPhonetics(true);
    }

    private string GetZhuyin()
    {
        return getPhonetics(false);
    }

    private string GetSinoVietnamese()
    {
        var nodes = _viHtmlDocument.QuerySelector(".detail-word .txt-word-multi .txt-cn_vi");
        return nodes != null && nodes.InnerText.Length > 2
            ? nodes.InnerText.Substring(1, nodes.InnerText.Length - 2)
            : string.Empty;
    }


    private string GetAudioURL(bool isMale)
    {
        var classes = _viHtmlDocument.QuerySelector(".detail-word .box-detail .box-detail-wrap").GetClasses();
        if (classes.Any())
        {
            var wordID = 0;
            foreach (var _class in classes)
            {
                if (int.TryParse(_class, out wordID))
                {
                    break;
                }
            }

            if (isMale)
            {
                return $"https://hanzii.net/audios/cnvi/1/{wordID}.mp3";
            }
            else
            {
                return $"https://hanzii.net/audios/cnvi/0/{wordID}.mp3";
            }
        }
        else
            return String.Empty;
    }

    private string GetAudioMaleURL()
    {
        return GetAudioURL(true);
    }

    private string GetAudioFemaleURL()
    {
        return GetAudioURL(false);
    }

    private List<string> GetLevels()
    {
        return _viHtmlDocument.QuerySelectorAll(".detail-word .word-level .tags").Select(node => node.InnerText).ToList();
    }

    private string GetClassifier()
    {
        var classifierText = String.Join("", _viHtmlDocument.QuerySelectorAll(".detail-word .word-deco").Select(node => node.InnerText));
        return classifierText.StartsWith(": ") ? classifierText.Substring(2) : classifierText;
    }

    private List<Definiton> GetDefinitions(bool en)
    {
        var definitions = new List<Definiton>();
        var htmlDocument = (en) ? _enHtmlDocument : _viHtmlDocument;
        var definitionNodes = htmlDocument.QuerySelectorAll(".box-detail .box-content");
        foreach (var definitionNode in definitionNodes)
        {
            var definition = new Definiton();
            definition.PartOfSpeech = definitionNode.QuerySelector(".kind-word").InnerText.Trim();
            var items = definitionNode.QuerySelectorAll(".item-content");
            foreach (var item in items)
            {
                var sense = new Sense();
                sense.Translation = item.QuerySelector(".box-mean .txt-mean .cl-content")?.InnerText ?? String.Empty;
                sense.SenseInChinese = item.QuerySelector(".box-mean .txt-mean-explain")?.InnerText ?? String.Empty;
                var exampleNodes = item.QuerySelectorAll(".box-example .content-example");
                foreach (var exampleNode in exampleNodes)
                {
                    var example = new Example();
                    example.Sentence = exampleNode.QuerySelector(".ex-word")?.InnerText ?? String.Empty;
                    example.Pinyin = exampleNode.QuerySelector(".ex-phonetic")?.InnerText ?? String.Empty;
                    example.Translation = exampleNode.QuerySelector(".ex-mean")?.InnerText ?? String.Empty;
                    sense.Examples.Add(example);
                }
                definition.Senses.Add(sense);
            }
            definitions.Add(definition);
        }
        return definitions;
    }

    private string GetRadical()
    {
        return _viHtmlDocument.QuerySelector(".detail-word .txt-detail.word-type > span")?.InnerText ?? String.Empty;
    }
}
