using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using System.Reflection;

using PokeBattleDex.Contracts.Services;

namespace PokeBattleDex.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

    public ICommand MenuHelpAboutCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        MenuHelpAboutCommand = new AsyncRelayCommand(OnMenuHelpAbout);
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private async Task OnMenuHelpAbout()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version!;
        var versionString = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        var dialog = new ContentDialog
        {
            Title = "About PokéBattleDex",
            CloseButtonText = "OK",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = $"PokéBattleDex v{versionString}",
                        Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                    },
                    new TextBlock
                    {
                        Text = "A fast, native, modern Pokédex for Windows.",
                        TextWrapping = TextWrapping.Wrap,
                    },
                    new HyperlinkButton
                    {
                        Content = "Source code on GitHub",
                        NavigateUri = new Uri("https://github.com/merc1er/PokeBattleDex"),
                    },
                    new TextBlock
                    {
                        Text = "Sources",
                        Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    },
                    new HyperlinkButton
                    {
                        Content = "Pokémon sprites from PokeAPI/sprites",
                        NavigateUri = new Uri("https://github.com/PokeAPI/sprites"),
                    },
                    new TextBlock
                    {
                        Text = "Contact",
                        Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    },
                    new TextBlock
                    {
                        Text = "Takedown, copyright infringement, and other legal notices regarding this repository may be submitted to 3af83whud@mozmail.com, and I will promptly and fully cooperate in good faith in accordance with applicable law.\n\nYou can also open an issue on GitHub.",
                        TextWrapping = TextWrapping.Wrap,
                    },
                },
            },
        };

        // XamlRoot must be set for ContentDialog in WinUI 3
        dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        await dialog.ShowAsync();
    }
}
