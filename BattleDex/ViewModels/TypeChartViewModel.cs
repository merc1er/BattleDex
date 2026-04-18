using CommunityToolkit.Mvvm.ComponentModel;

using BattleDex.Contracts.Services;
using BattleDex.Contracts.ViewModels;
using BattleDex.Core.Models;

namespace BattleDex.ViewModels;

public partial class TypeChartViewModel : ObservableRecipient, INavigationAware
{
    // V1 stored (int)GenerationChart when 0..6 meant Gen3..Gen9. V2 uses the new 0..8 = Gen1..Gen9 mapping.
    private const string SelectedGenerationKeyV1 = "TypeChartSelectedGeneration";
    private const string SelectedGenerationKey = "TypeChartSelectedGenerationV2";
    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    public partial GenerationChart SelectedGeneration { get; set; } = GenerationChart.Gen9;

    private bool _generationLoaded;

    // ComboBox index: 0 = Gen 1 chart, 1 = Gen 2–5 chart, 2 = Gen 6+ chart
    public int SelectedGenerationIndex
    {
        get => SelectedGeneration switch
        {
            GenerationChart.Gen1 => 0,
            GenerationChart.Gen2 or GenerationChart.Gen3 or GenerationChart.Gen4 or GenerationChart.Gen5 => 1,
            _ => 2,
        };
        set
        {
            var newGen = value switch
            {
                0 => GenerationChart.Gen1,
                1 => GenerationChart.Gen5,
                _ => GenerationChart.Gen9,
            };
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
        if (!savedGen.HasValue)
        {
            var savedGenV1 = await _localSettingsService.ReadSettingAsync<int?>(SelectedGenerationKeyV1);
            if (savedGenV1.HasValue)
            {
                savedGen = savedGenV1.Value + 2;
            }
        }
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
