namespace PokeBattleDex.Core.Models;

/// <summary>
/// Represents a Pokémon type (e.g., Fire, Water, Grass).
/// </summary>
public class PokemonType
{
    public string Name { get; set; } = string.Empty;

    public override string ToString() => Name;
}
