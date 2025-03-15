using HanziiAnki.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;

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

    private async void Keyword_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var keyboardState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
        if (e.Key == VirtualKey.Enter && keyboardState.HasFlag(CoreVirtualKeyStates.Down))
        {
            ViewModel.AddToDeck();
            return;
        }

        if (e.Key == VirtualKey.Enter)
        {
            await ViewModel.LookUpAsync();
            return;
        }
        
    }
}
