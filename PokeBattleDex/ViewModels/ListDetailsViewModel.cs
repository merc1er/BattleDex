using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using PokeBattleDex.Contracts.ViewModels;
using PokeBattleDex.Core.Contracts.Services;
using PokeBattleDex.Core.Models;

namespace PokeBattleDex.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    [ObservableProperty]
    private PokemonSpecies? selected;

    public ObservableCollection<PokemonSpecies> PokemonItems { get; private set; } = new ObservableCollection<PokemonSpecies>();

    public ListDetailsViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        PokemonItems.Clear();

        var data = await _sampleDataService.GetPokemonDataAsync();

        foreach (var item in data)
        {
            PokemonItems.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        Selected ??= PokemonItems.First();
    }
}
