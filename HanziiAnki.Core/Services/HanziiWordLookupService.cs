﻿using System;
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
    private readonly HtmlDocument _htmlDocument;
    private readonly IWebScrappingService _webScrappingService;
    public HanziiWordLookupService()
    {
        _htmlDocument = new HtmlDocument();
        _webScrappingService = new SeleniumScappingService();
    }
    private string GetQueryURL(string keyword) => $"https://hanzii.net/search/word/{keyword}?hl=vi";

    public async Task<Card> GetWordDefinition(string keyword)
    {
        var card = new Card();
        _htmlDocument.LoadHtml(_webScrappingService.ScrapeWebsite(GetQueryURL(keyword), ".detail-word"));

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
        var definitionsTask = Task.Run(() => card.Definitions = GetDefinitions());

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
            definitionsTask);

        return card;
    }

    private string GetSimplified()
    {
        try
        {
            return String.Join("", _htmlDocument.QuerySelectorAll(".query-search .simple-tradition-wrap > .cl-item").Select(node => node.InnerText));
        }
        catch (NullReferenceException)
        {
            return String.Empty;
        }

    }

    private string GetTraditional()
    {
        var traditionalNode = _htmlDocument.QuerySelectorAll(".query-search .simple-tradition-wrap > .wrap-convert > .cl-item");
        if (traditionalNode.Count == 0)
        {
            return GetSimplified();
        }
        return String.Join("", _htmlDocument.QuerySelectorAll(".query-search .simple-tradition-wrap > .wrap-convert > .cl-item").Select(node => node.InnerText));
    }

    private string getPhonetics(bool isPinyin)
    {
        try
        {
            var phoneticNodes = _htmlDocument.QuerySelectorAll(".detail-word .txt-word-multi .txt-pinyin");
            var phoneticText = isPinyin ? phoneticNodes[0].InnerText : phoneticNodes[1].InnerText;

            return phoneticText.Substring(1, phoneticText.Length - 2);
        }
        catch (NullReferenceException)
        {
            return String.Empty;
        }
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
        try
        {
            var sino_vie = _htmlDocument.QuerySelector(".detail-word .txt-word-multi .txt-cn_vi").InnerText;
            return sino_vie.Substring(1, sino_vie.Length - 2);
        }
        catch (NullReferenceException)
        {
            return String.Empty;
        }
    }

    private string GetAudioURL(bool isMale)
    {
        try
        {
            var classes = _htmlDocument.QuerySelector(".detail-word .box-detail .box-detail-wrap").GetClasses();
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
        catch (NullReferenceException)
        {
            return String.Empty;
        }
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
        return _htmlDocument.QuerySelectorAll(".detail-word .word-level .tags").Select(node => node.InnerText).ToList();
    }

    private string GetClassifier()
    {
        var classifierText = String.Join("", _htmlDocument.QuerySelectorAll(".detail-word .word-deco").Select(node => node.InnerText));
        return classifierText.StartsWith(": ") ? classifierText.Substring(2) : classifierText;
    }

    private List<Definiton> GetDefinitions()
    {
        var definitions = new List<Definiton>();
        var definitionNodes = _htmlDocument.QuerySelectorAll(".box-detail .box-content");
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
        return _htmlDocument.QuerySelector(".detail-word .txt-detail.word-type > span")?.InnerText ?? String.Empty;
    }
}
