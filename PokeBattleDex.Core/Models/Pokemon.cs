namespace PokeBattleDex.Core.Models;

/// <summary>
/// Represents a specific Pokémon form (e.g., Charizard, Charizard-Mega-X).
/// </summary>
public class Pokemon
{
    public string Name { get; set; } = string.Empty;

    public List<PokemonType> Types { get; set; } = new();

    /// <summary>
    /// Gets the types as a comma-separated string.
    /// </summary>
    public string TypesDisplay => string.Join(", ", Types.Select(t => char.ToUpper(t.Name[0]) + t.Name[1..]));
}
