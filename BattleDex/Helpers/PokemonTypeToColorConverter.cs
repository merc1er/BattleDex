using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using BattleDex.Core.Models;
using Windows.UI;

namespace BattleDex.Helpers;

/// <summary>
/// Converts a PokemonType enum value to its associated color.
/// </summary>
public class PokemonTypeToColorConverter : IValueConverter
{
    private static readonly Dictionary<PokemonType, SolidColorBrush> BrushCache = new();
    private static readonly SolidColorBrush DefaultBrush = new(Colors.Gray);

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PokemonType type)
        {
            return GetOrCreateBrush(type);
        }
        return DefaultBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    private static SolidColorBrush GetOrCreateBrush(PokemonType type)
    {
        if (!BrushCache.TryGetValue(type, out var brush))
        {
            brush = new SolidColorBrush(GetTypeColor(type));
            BrushCache[type] = brush;
        }
        return brush;
    }

    /// <summary>
    /// Returns black or white depending on the luminance of the given color.
    /// </summary>
    public static Color GetContrastForeground(Color bg)
    {
        // Perceived luminance formula
        var luminance = 0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B;
        return luminance > 150 ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
    }

    public static Color GetTypeColor(PokemonType type) => type switch
    {
        PokemonType.Normal => Color.FromArgb(255, 168, 167, 122),
        PokemonType.Fire => Color.FromArgb(255, 238, 129, 48),
        PokemonType.Water => Color.FromArgb(255, 99, 144, 240),
        PokemonType.Grass => Color.FromArgb(255, 122, 199, 76),
        PokemonType.Electric => Color.FromArgb(255, 247, 208, 44),
        PokemonType.Ice => Color.FromArgb(255, 150, 217, 214),
        PokemonType.Fighting => Color.FromArgb(255, 194, 46, 40),
        PokemonType.Poison => Color.FromArgb(255, 163, 62, 161),
        PokemonType.Ground => Color.FromArgb(255, 226, 191, 101),
        PokemonType.Flying => Color.FromArgb(255, 169, 143, 243),
        PokemonType.Psychic => Color.FromArgb(255, 249, 85, 135),
        PokemonType.Bug => Color.FromArgb(255, 166, 185, 26),
        PokemonType.Rock => Color.FromArgb(255, 182, 161, 54),
        PokemonType.Ghost => Color.FromArgb(255, 115, 87, 151),
        PokemonType.Dragon => Color.FromArgb(255, 111, 53, 252),
        PokemonType.Dark => Color.FromArgb(255, 112, 87, 70),
        PokemonType.Steel => Color.FromArgb(255, 183, 183, 206),
        PokemonType.Fairy => Color.FromArgb(255, 214, 133, 173),
        _ => Colors.Gray
    };
}
