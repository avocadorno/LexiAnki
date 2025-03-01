using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanziiAnki.Core.Contracts.Services;
using HanziiAnki.Core.Services;

namespace HanziiAnki.ViewModels;

public partial class DefineViewModel : ObservableRecipient
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookUpCommand))]
    public partial string? Simplified
    {
        get; set;
    }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookUpCommand))]
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

    [ObservableProperty]
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

    private IWordLookUpService _wordLookUpService;

    private bool CanLookup() => !String.IsNullOrEmpty(Simplified) || !String.IsNullOrEmpty(Traditional);

    public DefineViewModel(IWordLookUpService wordLookUpService)
    {
        _wordLookUpService = wordLookUpService;
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
    }

    [RelayCommand(CanExecute = nameof(CanLookup))]
    public void LookUp()
    {
        var keyword = String.Empty;
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
        var deck = _wordLookUpService.GetWordDefinition(keyword);
        Simplified = deck.Simplfied;
        Traditional = deck.Traditional;
        Pinyin = deck.Pinyin;
        Zhuyin = deck.Zhuyin;
        AudioFemaleURL = deck.AudioFemaleURL;
        AudioMaleURL = deck.AudioMaleURL;
        SinoVietnamese = deck.SinoVietnamese;
        Levels = String.Join(" | ", deck.Levels);
    }

    [RelayCommand]
    public void AddToDeck()
    {
        ClearFields();
    }
}
