using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

using BattleDex.Contracts.Services;
using BattleDex.Contracts.ViewModels;
using BattleDex.Core.Contracts.Services;
using BattleDex.Core.Models;
using BattleDex.Helpers;

namespace BattleDex.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private const string SelectedGenerationKey = "SelectedGeneration";
    private const string SelectedDexTypeKey = "SelectedDexType";
    private readonly ISampleDataService _sampleDataService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly int _itemsPerPage = 25;

    [ObservableProperty]
    public partial PokemonSpecies? Selected
    {
        get; set;
    }

    [ObservableProperty]
    public partial GenerationChart SelectedGeneration { get; set; } = GenerationChart.Gen9;

    [ObservableProperty]
    public partial DexType SelectedDexType { get; set; } = DexType.National;

    private bool _settingsLoaded;

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
            if (_settingsLoaded)
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
    private Dictionary<int, PokemonSpecies> _speciesById = new();
    private IReadOnlyDictionary<int, IReadOnlyList<int>> _regionalDex = new Dictionary<int, IReadOnlyList<int>>();

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
        // Restore persisted settings
        var savedGen = await _localSettingsService.ReadSettingAsync<int?>(SelectedGenerationKey);
        if (savedGen.HasValue && Enum.IsDefined(typeof(GenerationChart), savedGen.Value))
        {
            SelectedGeneration = (GenerationChart)savedGen.Value;
            OnPropertyChanged(nameof(SelectedGenerationIndex));
        }

        var savedDexType = await _localSettingsService.ReadSettingAsync<int?>(SelectedDexTypeKey);
        if (savedDexType.HasValue && Enum.IsDefined(typeof(DexType), savedDexType.Value))
        {
            SelectedDexType = (DexType)savedDexType.Value;
            OnPropertyChanged(nameof(SelectedDexTypeIndex));
        }

        _settingsLoaded = true;

        // Load data on background thread to avoid blocking UI
        var data = await Task.Run(() => _sampleDataService.GetPokemonDataAsync());
        _allPokemonItems = data.ToList();
        _speciesById = _allPokemonItems.ToDictionary(p => p.Id);
        _regionalDex = await Task.Run(() => _sampleDataService.GetRegionalDexAsync());

        ApplyFilter();
    }

    partial void OnSelectedGenerationChanged(GenerationChart value)
    {
        ApplyFilter();
    }

    partial void OnSelectedDexTypeChanged(DexType value)
    {
        if (_settingsLoaded)
        {
            _ = _localSettingsService.SaveSettingAsync(SelectedDexTypeKey, (int)value);
        }
        OnPropertyChanged(nameof(SelectedDexTypeIndex));
        ApplyFilter();
    }

    public int SelectedDexTypeIndex
    {
        get => (int)SelectedDexType;
        set
        {
            if ((int)SelectedDexType != value && Enum.IsDefined(typeof(DexType), value))
            {
                SelectedDexType = (DexType)value;
                OnPropertyChanged();
            }
        }
    }

    private int MaxGenerationNumber => (int)SelectedGeneration + 3;

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
        var maxGen = MaxGenerationNumber;
        var regionalIds = SelectedDexType == DexType.Regional && _regionalDex.TryGetValue(maxGen, out var ids)
            ? ids
            : null;

        bool MatchesSearch(PokemonSpecies item)
        {
            if (string.IsNullOrWhiteSpace(normalizedSearch))
            {
                return true;
            }
            var normalizedEnglish = NormalizeString(item.NameEnglish);
            var normalizedFrench = NormalizeString(item.NameFrench);
            var idString = item.Id.ToString();
            return normalizedEnglish.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                   normalizedFrench.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                   idString == normalizedSearch;
        }

        List<PokemonSpecies> filtered;
        if (regionalIds is not null)
        {
            // Regional mode: iterate in regional dex order.
            filtered = new List<PokemonSpecies>(regionalIds.Count);
            foreach (var id in regionalIds)
            {
                if (_speciesById.TryGetValue(id, out var species) && MatchesSearch(species))
                {
                    filtered.Add(species);
                }
            }
        }
        else
        {
            filtered = _allPokemonItems
                .Where(item => item.Generation <= maxGen && MatchesSearch(item))
                .ToList();
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

        if (Selected is not null)
        {
            var outOfRange = regionalIds is not null
                ? !regionalIds.Contains(Selected.Id)
                : Selected.Generation > maxGen;
            if (outOfRange)
            {
                Selected = FilteredPokemonItems.FirstOrDefault();
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
