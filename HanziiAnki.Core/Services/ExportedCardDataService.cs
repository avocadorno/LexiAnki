using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Models;

namespace HanziiAnki.Core.Services;

public class ExportedCardDataService : ICardDataService
{
    private readonly List<ExportedCard> _cards;
    public ExportedCardDataService()
    {
        _cards = new List<ExportedCard>();
    }
    public async Task SaveCardAsync(ExportedCard card)
    {
        await Task.Run(() => _cards.Add(card));
    }

    public async Task<IEnumerable<ExportedCard>> GetListDetailsDataAsync()
    {
        return await Task.FromResult(_cards.AsEnumerable());
    }
}