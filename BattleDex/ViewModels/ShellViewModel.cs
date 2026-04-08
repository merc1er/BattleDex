using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using System.Reflection;

using BattleDex.Contracts.Services;
using BattleDex.Helpers;

namespace BattleDex.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsBackEnabled
    {
        get; set;
    }

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
        var versionString = $"{version.Major}.{version.Minor}.{version.Build}";

        var dialog = new ContentDialog
        {
            Title = "About_Title".GetLocalized(),
            CloseButtonText = "About_CloseButton".GetLocalized(),
            Content = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = $"BattleDex v{versionString}",
                            Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                        },
                        new TextBlock
                        {
                            Text = "About_Description".GetLocalized(),
                            TextWrapping = TextWrapping.Wrap,
                        },
                        new HyperlinkButton
                        {
                            Content = "About_SourceCode".GetLocalized(),
                            NavigateUri = new Uri("https://github.com/merc1er/BattleDex"),
                        },
                        new TextBlock
                        {
                            Text = "About_SourcesHeader".GetLocalized(),
                            Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                        },
                        new HyperlinkButton
                        {
                            Content = "About_Sprites".GetLocalized(),
                            NavigateUri = new Uri("https://github.com/PokeAPI/sprites"),
                        },
                        new TextBlock
                        {
                            Text = "About_ContactHeader".GetLocalized(),
                            Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                        },
                        new TextBlock
                        {
                            Text = "About_ContactText".GetLocalized(),
                            TextWrapping = TextWrapping.Wrap,
                        },
                    },
                },
            },
        };

        // XamlRoot must be set for ContentDialog in WinUI 3
        dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        await dialog.ShowAsync();
    }
}
