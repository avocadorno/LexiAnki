﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Models;
using HtmlAgilityPack;

namespace HanziiAnki.Core.Contracts.Services;

public interface IWordLookUpService
{
    public Task<Card> GetWordDefinition(string keyword);
}