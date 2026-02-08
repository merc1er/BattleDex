using Microsoft.UI.Xaml.Data;
using PokeBattleDex.Core.Models;

namespace PokeBattleDex.Helpers;

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
