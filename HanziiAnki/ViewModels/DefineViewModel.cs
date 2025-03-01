using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Helpers;
using HanziiAnki.Core.Services;

namespace HanziiAnki.ViewModels;

public partial class DefineViewModel : ObservableRecipient
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookUpCommand)), NotifyCanExecuteChangedFor(nameof(AddToDeckCommand))]
    public partial string? Simplified
    {
        get; set;
    }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookUpCommand)), NotifyCanExecuteChangedFor(nameof(AddToDeckCommand))]
    public partial string? Traditional
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Pinyin
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Zhuyin
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? AudioMaleURL
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? AudioFemaleURL
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? SinoVietnamese
    {
        get; set;
    }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddToDeckCommand))]
    public partial string? Definition
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Levels
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Classifier
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Radical
    {
        get; set;
    }

    private readonly IWordLookUpService _wordLookUpService;
    private readonly ICardDataService _exportedCardDataService;

    private bool CanLookup() => !String.IsNullOrEmpty(Simplified) || !String.IsNullOrEmpty(Traditional);
    private bool CanAddToDeck() => (!String.IsNullOrEmpty(Simplified) || !String.IsNullOrEmpty(Traditional)) && !String.IsNullOrEmpty(Definition);

    public DefineViewModel(IWordLookUpService wordLookUpService, ICardDataService cardDataService)
    {
        _wordLookUpService = wordLookUpService;
        _exportedCardDataService = cardDataService;
    }

    [RelayCommand]
    public void ClearFields()
    {
        Simplified = String.Empty;
        Traditional = String.Empty;
        Pinyin = String.Empty;
        Zhuyin = String.Empty;
        AudioFemaleURL = String.Empty;
        AudioMaleURL = String.Empty;
        SinoVietnamese = String.Empty;
        Levels = String.Empty;
        Definition = String.Empty;
        Classifier = String.Empty;
        Radical = String.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanLookup))]
    public async Task LookUpAsync()
    {
        string? keyword;
        if (!String.IsNullOrEmpty(Traditional))
        {
            keyword = Traditional;
        }
        else if (!String.IsNullOrEmpty(Simplified))
        {
            keyword = Simplified;
        }
        else
        {
            return;
        }
        var card = await _wordLookUpService.GetWordDefinition(keyword);

        Simplified = card.Simplfied;
        Traditional = card.Traditional;
        Pinyin = card.Pinyin;
        Zhuyin = card.Zhuyin;
        AudioFemaleURL = card.AudioFemaleURL;
        AudioMaleURL = card.AudioMaleURL;
        SinoVietnamese = card.SinoVietnamese;
        Radical = card.Radical;
        Levels = card.GetLevelsAsString();
        Definition = card.GetMaskedDefintionAsString();
        Classifier = card.Classifier;
    }

    [RelayCommand(CanExecute = nameof(CanAddToDeck))]
    public void AddToDeck()
    {
        _exportedCardDataService.SaveCardAsync(new Core.Models.ExportedCard
        {
            Simplified = Simplified,
            Traditional = Traditional,
            Pinyin = Pinyin,
            Zhuyin = Zhuyin,
            AudioFemaleURL = AudioFemaleURL,
            AudioMaleURL = AudioMaleURL,
            SinoVietnamese = SinoVietnamese,
            Levels = Levels,
            Classifier = Classifier,
            Radical = Radical,
            Definitions = Definition
        });
        ClearFields();
    }
}
