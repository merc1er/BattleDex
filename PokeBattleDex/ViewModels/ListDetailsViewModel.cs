using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

using PokeBattleDex.Contracts.Services;
using PokeBattleDex.Contracts.ViewModels;
using PokeBattleDex.Core.Contracts.Services;
using PokeBattleDex.Core.Models;
using PokeBattleDex.Helpers;

namespace PokeBattleDex.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private const string SelectedGenerationKey = "SelectedGeneration";
    private readonly ISampleDataService _sampleDataService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly int _itemsPerPage = 25;

    [ObservableProperty]
    private PokemonSpecies? selected;

    [ObservableProperty]
    private GenerationChart selectedGeneration = GenerationChart.Gen6Plus;

    private bool _generationLoaded;

    public int SelectedGenerationIndex
    {
        get => (int)SelectedGeneration;
        set
        {
            if ((int)SelectedGeneration != value)
            {
                SelectedGeneration = (GenerationChart)value;
                OnPropertyChanged();
            }
            if (_generationLoaded)
            {
                _ = _localSettingsService.SaveSettingAsync(SelectedGenerationKey, value);
            }
        }
    }

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

    public ListDetailsViewModel(ISampleDataService sampleDataService, ILocalSettingsService localSettingsService)
    {
        _sampleDataService = sampleDataService;
        _localSettingsService = localSettingsService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        // Restore persisted generation selection
        try
        {
            var savedGen = await _localSettingsService.ReadSettingAsync<int>(SelectedGenerationKey);
            if (Enum.IsDefined(typeof(GenerationChart), savedGen))
            {
                SelectedGeneration = (GenerationChart)savedGen;
                OnPropertyChanged(nameof(SelectedGenerationIndex));
            }
        }
        catch
        {
            // First launch — no saved value yet
        }
        _generationLoaded = true;

        // Load data on background thread to avoid blocking UI
        var data = await Task.Run(() => _sampleDataService.GetPokemonDataAsync());
        _allPokemonItems = data.ToList();

        // Use incremental loading - only load first batch, rest loads on scroll
        var incrementalCollection = new IncrementalLoadingCollection<PokemonSpecies>(_allPokemonItems, batchSize: _itemsPerPage);
        incrementalCollection.LoadInitialItems();

        FilteredPokemonItems = incrementalCollection;
        OnPropertyChanged(nameof(FilteredPokemonItems));
        OnPropertyChanged(nameof(PokemonItems));
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

        IList<PokemonSpecies> filtered;

        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            filtered = _allPokemonItems;
        }
        else
        {
            filtered = _allPokemonItems.Where(item =>
            {
                var normalizedEnglish = NormalizeString(item.NameEnglish);
                var normalizedFrench = NormalizeString(item.NameFrench);

                return normalizedEnglish.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                       normalizedFrench.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase);
            }).ToList();
        }

        // Use incremental loading for large result sets, regular collection for small ones
        if (filtered.Count > 100)
        {
            var incrementalCollection = new IncrementalLoadingCollection<PokemonSpecies>(filtered, batchSize: _itemsPerPage);
            incrementalCollection.LoadInitialItems();
            FilteredPokemonItems = incrementalCollection;
        }
        else
        {
            FilteredPokemonItems = new ObservableCollection<PokemonSpecies>(filtered);
        }

        OnPropertyChanged(nameof(FilteredPokemonItems));
        OnPropertyChanged(nameof(PokemonItems));

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
