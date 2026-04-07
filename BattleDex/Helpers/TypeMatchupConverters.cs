using Microsoft.UI.Xaml.Data;
using BattleDex.Core.Models;

namespace BattleDex.Helpers;

/// <summary>
/// Converts a <see cref="TypeMultiplier"/> to a display string like "Fire ×2".
/// </summary>
public class TypeMultiplierToDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TypeMultiplier tm)
        {
            return $"{tm.Type} {tm.MultiplierDisplay}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// Returns a contrasting foreground brush (black or white) for a PokemonType background.
/// </summary>
public class PokemonTypeToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PokemonType type)
        {
            var bg = PokemonTypeToColorConverter.GetTypeColor(type);
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(PokemonTypeToColorConverter.GetContrastForeground(bg));
        }
        return new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// Returns a contrasting foreground brush (black or white) for a TypeMultiplier background.
/// </summary>
public class TypeMultiplierToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TypeMultiplier tm)
        {
            var bg = PokemonTypeToColorConverter.GetTypeColor(tm.Type);
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(PokemonTypeToColorConverter.GetContrastForeground(bg));
        }
        return new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// Extracts the PokemonType from a <see cref="TypeMultiplier"/> and converts it to a color.
/// </summary>
public class TypeMultiplierToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TypeMultiplier tm)
        {
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(PokemonTypeToColorConverter.GetTypeColor(tm.Type));
        }
        return new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
