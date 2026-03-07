using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using PokeBattleDex.Core.Models;
using PokeBattleDex.ViewModels;

namespace PokeBattleDex.Views;

public sealed partial class ListDetailsDetailControl : UserControl
{
    private readonly ListDetailsViewModel _viewModel;

    public PokemonSpecies? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as PokemonSpecies;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public TypeMatchup? CurrentMatchup =>
        ListDetailsMenuItem is { } item
            ? TypeEffectiveness.GetDefensiveMatchup(item.Types, _viewModel.SelectedGeneration)
            : null;

    public Visibility HasImmunities =>
        CurrentMatchup?.Immunities.Count > 0
            ? Visibility.Visible
            : Visibility.Collapsed;

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(PokemonSpecies), typeof(ListDetailsDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public ListDetailsDetailControl()
    {
        _viewModel = App.GetService<ListDetailsViewModel>();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        InitializeComponent();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ListDetailsViewModel.SelectedGeneration))
        {
            Bindings.Update();
        }
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListDetailsDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
            control.Bindings.Update();
        }
    }
}
