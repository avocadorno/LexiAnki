using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HanziiAnki.ViewModels;

public partial class DefineViewModel : ObservableRecipient
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookUpCommand))]
    public partial string? Simplified   {
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
    public partial string? Level
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Classifier
    {
        get; set;
    }

    private bool CanLookup() => !String.IsNullOrEmpty(Simplified) || !String.IsNullOrEmpty(Traditional);

    public DefineViewModel()
    {
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
        Level = String.Empty;
        Definition = String.Empty;
        Classifier = String.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanLookup))]
    public void LookUp()
    {
        
    }

    [RelayCommand]
    public void AddToDeck()
    {
        ClearFields();
    }
}
