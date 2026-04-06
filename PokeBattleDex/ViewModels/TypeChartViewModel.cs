using CommunityToolkit.Mvvm.ComponentModel;

using PokeBattleDex.Contracts.Services;
using PokeBattleDex.Contracts.ViewModels;
using PokeBattleDex.Core.Models;

namespace PokeBattleDex.ViewModels;

public partial class TypeChartViewModel : ObservableRecipient, INavigationAware
{
    private const string SelectedGenerationKey = "SelectedGeneration";
    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    public partial GenerationChart SelectedGeneration { get; set; } = GenerationChart.Gen9;

    private bool _generationLoaded;

    // ComboBox index: 0 = Gen 2–5 chart, 1 = Gen 6+ chart
    public int SelectedGenerationIndex
    {
        get => SelectedGeneration is GenerationChart.Gen3 or GenerationChart.Gen4 or GenerationChart.Gen5 ? 0 : 1;
        set
        {
            var newGen = value == 0 ? GenerationChart.Gen5 : GenerationChart.Gen9;
            if (SelectedGeneration != newGen)
            {
                SelectedGeneration = newGen;
                OnPropertyChanged();
            }
            if (_generationLoaded)
            {
                _ = _localSettingsService.SaveSettingAsync(SelectedGenerationKey, (int)newGen);
            }
        }
    }

    public TypeChartViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        var savedGen = await _localSettingsService.ReadSettingAsync<int?>(SelectedGenerationKey);
        if (savedGen.HasValue && Enum.IsDefined(typeof(GenerationChart), savedGen.Value))
        {
            SelectedGeneration = (GenerationChart)savedGen.Value;
            OnPropertyChanged(nameof(SelectedGenerationIndex));
        }
        _generationLoaded = true;
    }

    public void OnNavigatedFrom()
    {
    }
}
