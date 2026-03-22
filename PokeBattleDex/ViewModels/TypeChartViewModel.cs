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
