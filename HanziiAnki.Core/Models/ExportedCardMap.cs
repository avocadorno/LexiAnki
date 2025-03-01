using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace HanziiAnki.Core.Models;

public sealed class ExportedCardMap : ClassMap<ExportedCard>
{
    public ExportedCardMap()
    {
        Map(c => c.Simplified);
        Map(c => c.Traditional);
        Map(c => c.Pinyin);
        Map(c => c.Zhuyin);
        Map(c => c.SinoVietnamese);
        Map(c => c.Levels);
        Map(c => c.Classifier);
        Map(c => c.Radical);
        Map(c => c.FileTag);
        Map(c => c.Definitions);
    }
}
