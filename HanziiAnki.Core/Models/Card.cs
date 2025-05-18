using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HanziiAnki.Core.Helpers;

namespace HanziiAnki.Core.Models;

public class Example
{
    public string Sentence
    {
        get; set;
    }

    public string Translation
    {
        get; set;
    }

    public string Pinyin
    {
        get; set;
    }

    public override string ToString()
    {
        var parts = new List<string> { Sentence, Pinyin, Translation }
            .Where(str => !string.IsNullOrEmpty(str))
            .Select(str => HTMLHelper.GetWrapped(str, "div"));

        return string.Join("\n", parts);
    }
}

public class Sense
{
    public Sense()
    {
        Examples = new List<Example>();
    }

    public string Translation
    {
        get; set;
    }

    public string SenseInChinese
    {
        get; set;
    }

    public List<Example> Examples
    {
        get; set;
    }

    public override string ToString()
    {
        var parts = new List<string> { HTMLHelper.GetWrapped(HTMLHelper.GetBold(Translation), "div") };
        parts.Add(HTMLHelper.GetUnorderedList([.. Examples.Select(example => example.ToString())]));
        return string.Join("\n", parts);
    }
}

public class Definiton
{
    public Definiton()
    {
        Senses = new List<Sense>();
    }
    public string PartOfSpeech
    {
        get; set;
    }

    public List<Sense> Senses
    {
        get; set;
    }

    public override string ToString()
    {
        return HTMLHelper.GetItalic(PartOfSpeech.ToUpper()) + "\n" + HTMLHelper.GetOrderedList(Senses.Select(sense => sense.ToString()).ToList());
    }
}

public class Card
{
    public string Simplfied
    {
        get; set;
    }
    public string Traditional
    {
        get; set;
    }
    public string Pinyin
    {
        get; set;
    }

    public string Zhuyin
    {
        get; set;
    }

    public string AudioMaleURL
    {
        get; set;
    }

    public string AudioFemaleURL
    {
        get; set;
    }

    public string SinoVietnamese
    {
        get; set;
    }

    public List<String> Levels
    {
        get; set;
    }

    public string Classifier
    {
        get; set;
    }

    public string Radical
    {
        get; set;
    }

    public List<Definiton> EnDefinitions
    {
        get; set;
    }

    public List<Definiton> ViDefinitions
    {
        get; set;
    }

    public string GetLevelsAsString() => Levels.Any() ? String.Join(" | ", Levels) : String.Empty;
    
    public string GetDefinitionsAsString()
    {
        return HTMLHelper.GetBeautified(String.Join("\n", EnDefinitions)) + "<hr>\n" + HTMLHelper.GetBeautified(String.Join("\n", ViDefinitions));
    }
    public string GetMaskedDefintionAsString()
    {
        var maskedDefinitions = GetDefinitionsAsString();
        var maskedSimplified = String.Concat(Simplfied.Select(c => $"#{c}#"));
        var maskedTraditional = String.Concat(Traditional.Select(c => $"#{c}#"));
        var maskedPinyin = String.Join(" ", Pinyin.Split(" ").Select(word => $"#{word}#"));

        if (Simplfied.Equals(Traditional))
        {
            return maskedDefinitions.Replace(Traditional, maskedTraditional).Replace(Pinyin, maskedPinyin);
        }

        if (!String.IsNullOrEmpty(Simplfied))
            maskedDefinitions = maskedDefinitions.Replace(Simplfied, maskedSimplified);
        if (!String.IsNullOrEmpty(maskedTraditional))
            maskedDefinitions = maskedDefinitions.Replace(Traditional, maskedTraditional);
        if (!String.IsNullOrEmpty(Pinyin))
            maskedDefinitions = maskedDefinitions.Replace(Pinyin, maskedPinyin);
        return maskedDefinitions;
    }
}