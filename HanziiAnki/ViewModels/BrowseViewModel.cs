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

    private async Task<bool> DownloadFileAsync(HttpClient client, string url, string filePath)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            using FileStream fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 8192,
                useAsync: true);

            await contentStream.CopyToAsync(fileStream);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
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
        var downloadTasks = new List<Task<bool>>();

        foreach (var card in await _exportedCardDataService.GetListDetailsDataAsync())
        {
            if (!String.IsNullOrEmpty(card.AudioMaleURL) && !String.IsNullOrEmpty(card.AudioFemaleURL))
            {
                Random rand = new Random();
                var maleAudio = rand.Next(0, 2) == 1;
                downloadTasks.Add(DownloadFileAsync(client, (maleAudio) ? card.AudioMaleURL : card.AudioFemaleURL, downloadFolder + card.FileName));
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
