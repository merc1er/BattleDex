namespace BattleDex.Core.Models;

/// <summary>
/// Which Pokédex listing to show for the selected generation.
/// </summary>
public enum DexType
{
    /// <summary>All Pokémon up to and including the selected generation.</summary>
    National,

    /// <summary>Only Pokémon introduced in the selected generation.</summary>
    Regional,
}
