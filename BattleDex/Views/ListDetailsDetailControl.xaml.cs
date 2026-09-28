using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

using BattleDex.Core.Models;
using BattleDex.ViewModels;

namespace BattleDex.Views;

public sealed partial class ListDetailsDetailControl : UserControl
{
    private readonly ListDetailsViewModel _viewModel;
    private ListDetailsView? _listDetailsView;

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

    public bool IsShiny
    {
        get => (bool)GetValue(IsShinyProperty);
        set => SetValue(IsShinyProperty, value);
    }

    public string DisplayedSpriteUri =>
        ListDetailsMenuItem is { } item
            ? (IsShiny ? item.ShinySpriteUri : item.SpriteUri)
            : string.Empty;

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(PokemonSpecies), typeof(ListDetailsDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public static readonly DependencyProperty IsShinyProperty = DependencyProperty.Register("IsShiny", typeof(bool), typeof(ListDetailsDetailControl), new PropertyMetadata(false, OnIsShinyPropertyChanged));

    public ListDetailsDetailControl()
    {
        _viewModel = App.GetService<ListDetailsViewModel>();
        InitializeComponent();
        Loaded += ListDetailsDetailControl_Loaded;
        Unloaded += ListDetailsDetailControl_Unloaded;
    }

    private void ListDetailsDetailControl_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _listDetailsView = this.FindAscendant<ListDetailsView>();
        if (_listDetailsView is not null)
        {
            _listDetailsView.ViewStateChanged += OnViewStateChanged;
        }
        UpdateMatchupBindings();
        UpdateHeaderLayout();
    }

    private void ListDetailsDetailControl_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        if (_listDetailsView is not null)
        {
            _listDetailsView.ViewStateChanged -= OnViewStateChanged;
            _listDetailsView = null;
        }
    }

    private void OnViewStateChanged(object? sender, ListDetailsViewState e) => UpdateHeaderLayout();

    /// <summary>
    /// When only the details pane is visible, shows the back button and moves the Links button
    /// up beside it, above the name.
    /// </summary>
    private void UpdateHeaderLayout()
    {
        var detailsOnly = _listDetailsView?.ViewState == ListDetailsViewState.Details;
        BackButton.Visibility = detailsOnly ? Visibility.Visible : Visibility.Collapsed;

        Grid.SetRow(LinksButton, detailsOnly ? 0 : 1);
        Grid.SetColumn(LinksButton, detailsOnly ? 0 : 3);
        Grid.SetColumnSpan(LinksButton, detailsOnly ? 4 : 1);
        LinksButton.HorizontalAlignment = detailsOnly ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        LinksButton.Margin = (Thickness)Resources[detailsOnly ? "LinksAboveNameMargin" : "LinksBesideNameMargin"];
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Selected = null;
    }
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ListDetailsViewModel.SelectedGeneration))
        {
            UpdateMatchupBindings();
        }
    }

    private void UpdateMatchupBindings()
    {
        var matchup = CurrentMatchup;
        WeaknessesControl.ItemsSource = matchup?.Weaknesses;
        ResistancesControl.ItemsSource = matchup?.Resistances;
        ImmunitiesControl.ItemsSource = matchup?.Immunities;
        var immuneVis = matchup?.Immunities.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        ImmunitiesHeader.Visibility = immuneVis;
        ImmunitiesControl.Visibility = immuneVis;
        EvYieldText.Text = ListDetailsMenuItem?.GetEvYieldDisplay(_viewModel.SelectedGeneration) ?? string.Empty;
        SmogonLinkItem.Visibility = ListDetailsMenuItem?.GetSmogonUri(_viewModel.SelectedGeneration) is not null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private async void PokemonDbLink_Click(object sender, RoutedEventArgs e)
    {
        if (ListDetailsMenuItem is { } item)
        {
            await Launcher.LaunchUriAsync(item.PokemonDbUri);
        }
    }

    private async void BulbapediaLink_Click(object sender, RoutedEventArgs e)
    {
        if (ListDetailsMenuItem is { } item)
        {
            await Launcher.LaunchUriAsync(item.BulbapediaUri);
        }
    }

    private async void SmogonLink_Click(object sender, RoutedEventArgs e)
    {
        if (ListDetailsMenuItem?.GetSmogonUri(_viewModel.SelectedGeneration) is { } uri)
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListDetailsDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
            control.IsShiny = false;
            control.Bindings.Update();
            control.UpdateMatchupBindings();
            control.UpdateHeaderLayout();
        }
    }

    private static void OnIsShinyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListDetailsDetailControl control)
        {
            control.Bindings.Update();
        }
    }
}
