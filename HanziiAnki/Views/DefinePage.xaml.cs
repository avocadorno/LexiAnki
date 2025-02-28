using HanziiAnki.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace HanziiAnki.Views;

public sealed partial class DefinePage : Page
{
    public DefineViewModel ViewModel
    {
        get;
    }

    public DefinePage()
    {
        ViewModel = App.GetService<DefineViewModel>();
        InitializeComponent();
    }
}
