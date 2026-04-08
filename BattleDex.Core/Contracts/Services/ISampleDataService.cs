using BattleDex.Core.Models;

namespace BattleDex.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface ISampleDataService
{
    Task<IEnumerable<PokemonSpecies>> GetPokemonDataAsync();
}
