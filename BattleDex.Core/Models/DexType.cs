namespace BattleDex.Core.Models;

/// <summary>
/// Which Pokédex listing to show for the selected generation.
/// </summary>
public enum DexType
{
    /// <summary>All Pokémon up to and including the selected generation.</summary>
    National,

    /// <summary>
    /// Pokémon in the selected generation's regional Pokédex, in regional dex order.
    /// Includes species from earlier generations that appear in that region.
    /// </summary>
    Regional,
}
