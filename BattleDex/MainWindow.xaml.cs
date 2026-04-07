using Microsoft.UI.Windowing;

using BattleDex.Contracts.Services;
using BattleDex.Helpers;

using Windows.Graphics;
using Windows.UI.ViewManagement;

namespace BattleDex;

public sealed partial class MainWindow : WindowEx
{
    private const string WindowSizeKey = "WindowSize";

    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private UISettings settings;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();

        // Hide window initially to prevent startup flash (WinUI 3 visual artifact
        // workaround).
        AppWindow.Hide();

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        // Save window size when closing
        AppWindow.Closing += AppWindow_Closing;
    }

    public async Task RestoreWindowSizeAsync()
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();
        var savedSettings = await localSettingsService.ReadSettingAsync<WindowSizeSettings>(WindowSizeKey);

        if (savedSettings != null && savedSettings.Width > 0 && savedSettings.Height > 0)
        {
            AppWindow.MoveAndResize(new RectInt32(savedSettings.X, savedSettings.Y, savedSettings.Width, savedSettings.Height));
        }
    }

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        // Defer closing to allow async save to complete
        args.Cancel = true;

        var localSettingsService = App.GetService<ILocalSettingsService>();
        var windowSettings = new WindowSizeSettings
        {
            Width = AppWindow.Size.Width,
            Height = AppWindow.Size.Height,
            X = AppWindow.Position.X,
            Y = AppWindow.Position.Y
        };

        await localSettingsService.SaveSettingAsync(WindowSizeKey, windowSettings);

        // Unhook event to prevent recursion.
        AppWindow.Closing -= AppWindow_Closing;

        // Hide window before closing to prevent black frame flash (WinUI 3 visual
        // artifact workaround).
        AppWindow.Hide();

        Close();
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}

public class WindowSizeSettings
{
    public int Width
    {
        get; set;
    }
    public int Height
    {
        get; set;
    }
    public int X
    {
        get; set;
    }
    public int Y
    {
        get; set;
    }
}
