using CommunityToolkit.WinUI.UI.Controls;

using HanziiAnki.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace HanziiAnki.Views;

public sealed partial class BrowsePage : Page
{
    public BrowseViewModel ViewModel
    {
        get;
    }

    public BrowsePage()
    {
        ViewModel = App.GetService<BrowseViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
