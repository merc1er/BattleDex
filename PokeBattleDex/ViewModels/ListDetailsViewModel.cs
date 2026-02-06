using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

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

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    private List<PokemonSpecies> _allPokemonItems = new();

    public ObservableCollection<PokemonSpecies> FilteredPokemonItems { get; private set; } = new();

    // Keep for backwards compatibility
    public ObservableCollection<PokemonSpecies> PokemonItems => FilteredPokemonItems;

    public ListDetailsViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        _allPokemonItems.Clear();
        FilteredPokemonItems.Clear();

        var data = await _sampleDataService.GetPokemonDataAsync();

        foreach (var item in data)
        {
            _allPokemonItems.Add(item);
            FilteredPokemonItems.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        Selected ??= FilteredPokemonItems.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        var normalizedSearch = NormalizeString(SearchText);

        FilteredPokemonItems.Clear();

        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            foreach (var item in _allPokemonItems)
            {
                FilteredPokemonItems.Add(item);
            }
        }
        else
        {
            foreach (var item in _allPokemonItems)
            {
                var normalizedEnglish = NormalizeString(item.NameEnglish);
                var normalizedFrench = NormalizeString(item.NameFrench);

                if (normalizedEnglish.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    normalizedFrench.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredPokemonItems.Add(item);
                }
            }
        }

        // Auto-select if there's exactly one match.
        if (FilteredPokemonItems.Count == 1)
        {
            Selected = FilteredPokemonItems[0];
        }
    }

    /// <summary>
    /// Normalizes a string by removing accents/diacritics and trimming whitespace.
    /// </summary>
    private static string NormalizeString(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Trim spaces
        text = text.Trim();

        // Remove accents by decomposing and filtering out diacritical marks
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
