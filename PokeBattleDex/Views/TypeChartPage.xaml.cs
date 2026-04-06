using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using PokeBattleDex.Core.Models;
using PokeBattleDex.Helpers;
using PokeBattleDex.ViewModels;

using Windows.UI;

namespace PokeBattleDex.Views;

public sealed partial class TypeChartPage : Page
{
    private const double CellSize = 40;
    private const double HeaderWidth = 80;

    public TypeChartViewModel ViewModel { get; }

    public TypeChartPage()
    {
        ViewModel = App.GetService<TypeChartViewModel>();
        InitializeComponent();
        Loaded += TypeChartPage_Loaded;
        Unloaded += TypeChartPage_Unloaded;
    }

    private void TypeChartPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        BuildChart();
    }

    private void TypeChartPage_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TypeChartViewModel.SelectedGeneration))
        {
            BuildChart();
        }
    }

    private void BuildChart()
    {
        ChartGrid.Children.Clear();
        ChartGrid.RowDefinitions.Clear();
        ChartGrid.ColumnDefinitions.Clear();

        var gen = ViewModel.SelectedGeneration;
        var types = GetVisibleTypes(gen);
        var count = types.Length;

        // Define rows and columns: header + N type rows/cols
        ChartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(HeaderWidth) });
        for (var i = 0; i < count; i++)
        {
            ChartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(CellSize) });
        }

        ChartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(HeaderWidth) });
        for (var i = 0; i < count; i++)
        {
            ChartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CellSize) });
        }

        // Corner cell — label
        var cornerText = new TextBlock
        {
            Text = "ATK ↓\nDEF ➜",
            FontSize = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            Foreground = new SolidColorBrush(Colors.Gray),
        };
        Grid.SetRow(cornerText, 0);
        Grid.SetColumn(cornerText, 0);
        ChartGrid.Children.Add(cornerText);

        // Column headers (defending types) — rotated text
        for (var i = 0; i < count; i++)
        {
            var type = types[i];
            var color = PokemonTypeToColorConverter.GetTypeColor(type);
            var fg = PokemonTypeToColorConverter.GetContrastForeground(color);

            var text = new TextBlock
            {
                Text = type.ToString(),
                FontSize = 10,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(fg),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5),
                RenderTransform = new RotateTransform { Angle = -60 },
            };

            var border = new Border
            {
                Background = new SolidColorBrush(color),
                Child = text,
            };

            Grid.SetRow(border, 0);
            Grid.SetColumn(border, i + 1);
            ChartGrid.Children.Add(border);
        }

        // Row headers (attacking types)
        for (var i = 0; i < count; i++)
        {
            var type = types[i];
            var color = PokemonTypeToColorConverter.GetTypeColor(type);
            var fg = PokemonTypeToColorConverter.GetContrastForeground(color);

            var text = new TextBlock
            {
                Text = type.ToString(),
                FontSize = 10,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(fg),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var border = new Border
            {
                Background = new SolidColorBrush(color),
                Padding = new Thickness(4, 0, 4, 0),
                Child = text,
            };

            Grid.SetRow(border, i + 1);
            Grid.SetColumn(border, 0);
            ChartGrid.Children.Add(border);
        }

        // Effectiveness cells
        for (var row = 0; row < count; row++)
        {
            var atkType = types[row];
            for (var col = 0; col < count; col++)
            {
                var defType = types[col];
                var multiplier = TypeEffectiveness.GetMultiplier(atkType, defType, gen);

                var (bgColor, fgColor, displayText) = GetCellStyle(multiplier);

                var text = new TextBlock
                {
                    Text = displayText,
                    FontSize = 11,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(fgColor),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var border = new Border
                {
                    Background = new SolidColorBrush(bgColor),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(30, 128, 128, 128)),
                    BorderThickness = new Thickness(0.5),
                    Child = text,
                };

                Grid.SetRow(border, row + 1);
                Grid.SetColumn(border, col + 1);
                ChartGrid.Children.Add(border);
            }
        }
    }

    private static PokemonType[] GetVisibleTypes(GenerationChart gen)
    {
        var allTypes = Enum.GetValues<PokemonType>();
        if (gen is GenerationChart.Gen3 or GenerationChart.Gen4 or GenerationChart.Gen5)
        {
            // Exclude Fairy (index 17) — didn't exist before Gen 6
            return allTypes.Where(t => (int)t < (int)PokemonType.Fairy).ToArray();
        }
        return allTypes;
    }

    private static (Color bg, Color fg, string text) GetCellStyle(float multiplier) => multiplier switch
    {
        0f => (Color.FromArgb(255, 50, 50, 50), Colors.White, "0"),
        0.25f => (Color.FromArgb(255, 190, 60, 60), Colors.White, "¼"),
        0.5f => (Color.FromArgb(255, 220, 100, 100), Colors.White, "½"),
        2f => (Color.FromArgb(255, 80, 170, 80), Colors.White, "2"),
        4f => (Color.FromArgb(255, 40, 130, 40), Colors.White, "4"),
        _ => (Color.FromArgb(255, 60, 60, 60), Color.FromArgb(100, 180, 180, 180), ""),
    };
}
