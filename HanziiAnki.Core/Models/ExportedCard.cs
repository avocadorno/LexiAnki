using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanziiAnki.Core.Models;

public class ExportedCard
{
    public string Simplified
    {
        get; set;
    }
    public string Traditional
    {
        get; set;
    }

    public string Simplified_Traditional => (Traditional == Simplified) ? Traditional : $"{Traditional}【{Simplified}】";
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
    public string FileName => AudioFemaleURL.Split("/").Last();
    public string FileTag => $"[sound:{FileName}]";
    public string SinoVietnamese
    {
        get; set;
    }

    public string Levels
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

    public string Definitions
    {
        get; set;
    }
}
