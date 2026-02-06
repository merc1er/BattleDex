using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using PokeBattleDex.Activation;
using PokeBattleDex.Contracts.Services;
using PokeBattleDex.Core.Contracts.Services;
using PokeBattleDex.Views;

namespace PokeBattleDex.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly ISampleDataService _sampleDataService;
    private UIElement? _shell = null;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, ISampleDataService sampleDataService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _sampleDataService = sampleDataService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
        }

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Restore window size/position before showing to prevent visual jump.
        await ((MainWindow)App.MainWindow).RestoreWindowSizeAsync();

        // Activate the MainWindow (hidden initially to prevent flash).
        App.MainWindow.Activate();

        // Allow UI to render a frame before showing window to prevent black flash.
        await Task.Delay(50);

        // Show window after content is ready to prevent startup flash (WinUI 3 visual
        // artifact workaround).
        App.MainWindow.AppWindow.Show();

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        // Preload Pokemon data on background thread while splash is shown
        await Task.Run(() => _sampleDataService.GetPokemonDataAsync());
    }

    private async Task StartupAsync()
    {
        await Task.CompletedTask;
    }
}
