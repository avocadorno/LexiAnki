using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HanziiAnki.ViewModels;

public partial class DefineViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string? _simplified;
    [ObservableProperty]
    private string? _traditional;
    [ObservableProperty]
    private string? _pinyin;
    [ObservableProperty]
    private string? _zhuyin;
    [ObservableProperty]
    private string? _audioMaleURL;
    [ObservableProperty]
    private string? _audioFemaleURL;
    [ObservableProperty]
    private string? _sinoVietnamese;
    [ObservableProperty]
    private string? _level;
    [ObservableProperty]
    private string? _definition;
    [ObservableProperty]
    private string? _classifier;

    public DefineViewModel()
    {
    }

    [RelayCommand]
    public void ClearFields()
    {
        _simplified = string.Empty;
        _traditional = string.Empty;
        _pinyin = string.Empty;
        _zhuyin = string.Empty;
        _audioMaleURL = string.Empty;
        _audioFemaleURL = string.Empty;
        _sinoVietnamese = string.Empty;
        _level = string.Empty;
        _definition = string.Empty;
        _classifier = string.Empty;
    }

    [RelayCommand]
    public void LookUp()
    {
    
    }

    [RelayCommand]
    public void AddToDeck()
    {
        ClearFields();
    }
}
