using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using PokeBattleDex.Core.Models;

namespace PokeBattleDex.Views;

public sealed partial class ListDetailsDetailControl : UserControl
{
    public PokemonSpecies? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as PokemonSpecies;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(PokemonSpecies), typeof(ListDetailsDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public ListDetailsDetailControl()
    {
        InitializeComponent();
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListDetailsDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
