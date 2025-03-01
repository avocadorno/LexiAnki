using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

public class Sense
{
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
}

public class Definiton
{
    public string PartOfSpeech
    {
        get; set;
    }

    public Sense Senses
    {
        get; set;
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

    public List<Definiton> Definitions
    {
        get; set;
    }
}
