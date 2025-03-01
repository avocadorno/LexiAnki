using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HanziiAnki.Core.Models;

namespace HanziiAnki.Core.Contracts.Services;

public interface ICardDataService
{
    Task SaveCardAsync(ExportedCard card);
    Task<IEnumerable<ExportedCard>> GetListDetailsDataAsync();
}
