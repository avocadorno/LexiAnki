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
    private const string ElementToWaitFor = ".box-detail-wrap";
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
        var characterNodes = _viHtmlDocument.QuerySelectorAll("#word .line-word .simple-tradition-wrap > .query-matched");
        return characterNodes.Any() ? String.Join("", characterNodes.Select(node => node.InnerText)) : String.Empty;
    }

    private string GetTraditional()
    {
        var characterNodes = _viHtmlDocument.QuerySelectorAll("#word .line-word .simple-tradition-wrap > .wrap-convert > .matched");
        return characterNodes.Any() ? String.Join("", characterNodes.Select(node => node.InnerText)) : GetSimplified();
    }
    
    private string getPhonetics(bool isPinyin)
    {
        var phoneticNodes = _viHtmlDocument.QuerySelectorAll("#word .line-word .txt-pinyin");
        var index = isPinyin ? 0 : 1;
        if (phoneticNodes.Count > index + 1)
        {
            var phoneticText = phoneticNodes[index].InnerText;
            return phoneticText.Length > 2 ? phoneticText.Substring(1, phoneticText.Length - 2).Trim() : string.Empty;
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
        var nodes = _viHtmlDocument.QuerySelector("#word .line-word .txt-cn_vi");
        return nodes != null && nodes.InnerText.Length > 2
            ? nodes.InnerText.Substring(1, nodes.InnerText.Length - 2).Trim()
            : string.Empty;
    }


    private string GetAudioURL(bool isMale)
    {
        var nodes = _viHtmlDocument.QuerySelector(".box-detail-wrap");
        if (nodes == null)
            return String.Empty;
        var classes = nodes.GetClasses();
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
        return _viHtmlDocument.QuerySelectorAll("#word .word-level .tags").Select(node => node.InnerText).ToList();
    }

    private string GetClassifier() => String.Join("", _viHtmlDocument.QuerySelectorAll("#measure .word-deco").Select(node => node.InnerText)).Trim();
    private string SwapBracketContent(string input)
    {
        var start = input.IndexOf('【');
        var end = input.IndexOf('】');

        if (start != -1 && end != -1 && start < end)
        {
            var traditional = input.Substring(start + 1, end - start - 1);
            var simplified = input.Substring(0, start) + input.Substring(end + 1);
            return $"{traditional}【{simplified}】";
        }

        return input;
    }

    private List<Definiton> GetDefinitions(bool en)
    {
        var definitions = new List<Definiton>();
        var htmlDocument = (en) ? _enHtmlDocument : _viHtmlDocument;
        var definitionNodes = htmlDocument.QuerySelectorAll(".box-detail-wrap .bg-inverse.content");
        foreach (var definitionNode in definitionNodes)
        {
            var definition = new Definiton();
            definition.PartOfSpeech = definitionNode.QuerySelector(".box-title").InnerText.Trim();
            var items = definitionNode.QuerySelectorAll(".content-item");
            foreach (var item in items)
            {
                var sense = new Sense();
                sense.Translation = item.QuerySelector(".box-mean .txt-mean .simple-tradition-wrap")?.InnerText ?? String.Empty;
                sense.SenseInChinese = item.QuerySelector(".box-example .txt-mean-explain")?.InnerText ?? String.Empty;
                var exampleNodes = item.QuerySelectorAll(".box-example example .box-example");
                foreach (var exampleNode in exampleNodes)
                {
                    var example = new Example();
                    example.Sentence = SwapBracketContent(exampleNode.QuerySelector(".simple-tradition-wrap")?.InnerText ?? String.Empty);
                    example.Pinyin = exampleNode.QuerySelector(".ex-phonetic")?.InnerText ?? String.Empty;
                    example.Translation = exampleNode.QuerySelectorAll("div").Last()?.InnerText ?? String.Empty;
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
        return _viHtmlDocument.QuerySelector("#word .word-compound .hanzi-sets span")?.InnerText ?? String.Empty;
    }
}
