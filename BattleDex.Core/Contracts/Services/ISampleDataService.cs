using BattleDex.Core.Models;

namespace BattleDex.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface ISampleDataService
{
    Task<IEnumerable<PokemonSpecies>> GetPokemonDataAsync();

    /// <summary>
    /// Returns a map from generation number (3-9) to the national dex IDs of that
    /// generation's regional Pokédex, ordered by regional dex position.
    /// </summary>
    Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetRegionalDexAsync();
}
