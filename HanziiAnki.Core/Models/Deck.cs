using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        var parts = new List<string> { HTMLHelper.GetBold(Translation), SenseInChinese }
            .Select(str => HTMLHelper.GetWrapped(str, "div"))
            .ToList();
        parts.Add(HTMLHelper.GetUnorderedList(Examples.Select(example => example.ToString()).ToList()));
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

public class ChineseDeck
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

    public List<Definiton> Definitions
    {
        get; set;
    }
}
