using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml.Controls;

using PokeBattleDex.ViewModels;

namespace PokeBattleDex.Views;

public sealed partial class ListDetailsPage : Page
{
    public ListDetailsViewModel ViewModel
    {
        get;
    }

    public ListDetailsPage()
    {
        ViewModel = App.GetService<ListDetailsViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }

    private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.SearchText = sender.Text;
        }
    }

    private void ListDetailsViewControl_GotFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Keep focus on search box when there's active search text
        if (!string.IsNullOrEmpty(ViewModel.SearchText))
        {
            SearchBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }
    }
}
