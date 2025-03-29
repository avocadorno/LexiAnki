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
    public partial ExportedCard? Selected
    {
        get; set;
    }

    public ObservableCollection<ExportedCard> ExportedCards { get; private set; } = new ObservableCollection<ExportedCard>();

    private bool CanExport() => ExportedCards.Any();

    public BrowseViewModel(ICardDataService cardDataService)
    {
        _exportedCardDataService = cardDataService;
    }

    private async Task DownloadFileAsync(HttpClient client, string url, string filePath)
    {
        using HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                       fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await contentStream.CopyToAsync(fileStream);
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    public async Task ExportAsync()
    {
        var downloadFolder = @"D:/Output/";
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        using HttpClient client = new HttpClient();
        var downloadTasks = new List<Task>();

        foreach (var card in await _exportedCardDataService.GetListDetailsDataAsync())
        {
            if (!String.IsNullOrEmpty(card.AudioMaleURL))
            {
                downloadTasks.Add(DownloadFileAsync(client, card.AudioMaleURL, downloadFolder + card.FileName));
            }
        }

        await Task.WhenAll(downloadTasks);

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
