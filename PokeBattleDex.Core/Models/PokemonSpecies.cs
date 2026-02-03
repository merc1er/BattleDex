namespace PokeBattleDex.Core.Models;

/// <summary>
/// Represents a Pokémon species.
/// </summary>
public class PokemonSpecies
{
    /// <summary>
    /// The species name (e.g., "bulbasaur").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The Pokédex number.
    /// </summary>
    public int Id
    {
        get; set;
    }

    /// <summary>
    /// The English name of the Pokémon.
    /// </summary>
    public string NameEnglish { get; set; } = string.Empty;

    /// <summary>
    /// The French name of the Pokémon.
    /// </summary>
    public string NameFrench { get; set; } = string.Empty;

    /// <summary>
    /// The Pokémon's types.
    /// </summary>
    public List<PokemonType> Types { get; set; } = new();

    /// <summary>
    /// Base stat total.
    /// </summary>
    public int Total
    {
        get; set;
    }

    /// <summary>
    /// Base HP stat.
    /// </summary>
    public int HP
    {
        get; set;
    }

    /// <summary>
    /// Base Attack stat.
    /// </summary>
    public int Attack
    {
        get; set;
    }

    /// <summary>
    /// Base Defense stat.
    /// </summary>
    public int Defense
    {
        get; set;
    }

    /// <summary>
    /// Base Special Attack stat.
    /// </summary>
    public int SpAtk
    {
        get; set;
    }

    /// <summary>
    /// Base Special Defense stat.
    /// </summary>
    public int SpDef
    {
        get; set;
    }

    /// <summary>
    /// Base Speed stat.
    /// </summary>
    public int Speed
    {
        get; set;
    }

    /// <summary>
    /// The generation this Pokémon was introduced in.
    /// </summary>
    public int Generation
    {
        get; set;
    }

    /// <summary>
    /// Whether this Pokémon is a legendary.
    /// </summary>
    public bool IsLegendary
    {
        get; set;
    }

    /// <summary>
    /// Gets the display name with the Pokédex number.
    /// </summary>
    public string DisplayName => $"#{Id:D3} {NameEnglish}";

    /// <summary>
    /// Gets the types as a display string.
    /// </summary>
    public string TypesDisplay => Types.Count > 0
        ? string.Join(", ", Types.Select(t => char.ToUpper(t.Name[0]) + t.Name[1..]))
        : string.Empty;

    /// <summary>
    /// Gets a symbol code based on the primary type for display purposes.
    /// </summary>
    public int SymbolCode
    {
        get
        {
            var primaryType = Types.FirstOrDefault()?.Name ?? "";
            return primaryType.ToLower() switch
            {
                "fire" => 0xE706,      // Brightness (sun-like)
                "water" => 0xE774,     // Drizzle/rain
                "grass" => 0xE8B3,     // Leaf
                "electric" => 0xE945,  // Lightning
                "poison" => 0xE7BA,    // Warning
                "flying" => 0xE709,    // Up arrow
                "dragon" => 0xE735,    // Star filled
                "normal" => 0xE73E,    // Circle
                "fighting" => 0xE74E,  // Heart (passion)
                "ground" => 0xE81C,    // Map
                "rock" => 0xE71D,      // Solid
                "bug" => 0xE774,       // Bug
                "ghost" => 0xE7B3,     // Eye
                "steel" => 0xE72E,     // Sync (metallic)
                "psychic" => 0xE7B3,   // Eye
                "ice" => 0xE9CA,       // Snow
                "dark" => 0xE708,      // Moon
                "fairy" => 0xE74E,     // Heart
                _ => 0xE73E            // Default circle
            };
        }
    }

    /// <summary>
    /// Gets the symbol as a string for display.
    /// </summary>
    public string Symbol => char.ConvertFromUtf32(SymbolCode);
}
