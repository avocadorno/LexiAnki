using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using CsvHelper.Configuration;
using HanziiAnki.Contracts.ViewModels;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Models;
using Windows.Devices.Usb;

namespace HanziiAnki.ViewModels;

public partial class BrowseViewModel : ObservableRecipient, INavigationAware
{
    private readonly ICardDataService _exportedCardDataService;

    [ObservableProperty]
    private ExportedCard? selected;

    public ObservableCollection<ExportedCard> ExportedCards { get; private set; } = new ObservableCollection<ExportedCard>();

    public BrowseViewModel(ICardDataService cardDataService)
    {
        _exportedCardDataService = cardDataService;
    }

    [RelayCommand]
    public async Task ExportAsync()
    {
        foreach (var card in await _exportedCardDataService.GetListDetailsDataAsync())
        {
            var folderPath = @"D:/Output/";

            // Check if the directory exists
            if (!Directory.Exists(folderPath))
            {
                // Create the directory if it does not exist
                Directory.CreateDirectory(folderPath);
            }

            var url = card.AudioFemaleURL;
            var filePath = folderPath + card.FileName;

            if (string.IsNullOrEmpty(url))
                continue;
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            await contentStream.CopyToAsync(fileStream);
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|"
        };

        using var writer = new StreamWriter("D:/Output/output.csv");
        using var csv = new CsvWriter(writer, config);
        csv.Context.RegisterClassMap<ExportedCardMap>();
        csv.WriteRecords(await _exportedCardDataService.GetListDetailsDataAsync());
    }

    public async void OnNavigatedTo(object parameter)
    {
        ExportedCards.Clear();

        var data = await _exportedCardDataService.GetListDetailsDataAsync();

        foreach (var item in data)
        {
            ExportedCards.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        if (ExportedCards != null && ExportedCards.Any())
        {
            Selected ??= ExportedCards.First();
        }
        else
        {
            Selected = null;
        }
    }

}
