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
    /// The Pokémon's primary ability.
    /// </summary>
    public string Ability1 { get; set; } = string.Empty;

    /// <summary>
    /// The Pokémon's secondary ability (if any).
    /// </summary>
    public string Ability2 { get; set; } = string.Empty;

    /// <summary>
    /// The Pokémon's hidden ability (if any).
    /// </summary>
    public string HiddenAbility { get; set; } = string.Empty;

    /// <summary>
    /// Gets the display name with the Pokédex number.
    /// </summary>
    public string DisplayName => $"#{Id:D3} {NameEnglish}";

    /// <summary>
    /// Base directory for sprite assets. Must be set by the application on startup.
    /// </summary>
    public static string SpriteBasePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the file path for the Pokémon's sprite image.
    /// </summary>
    public string SpriteUri
    {
        get
        {
            if (string.IsNullOrEmpty(SpriteBasePath))
            {
                return string.Empty;
            }
            var filePath = Path.Combine(SpriteBasePath, "Assets", "Sprites", "Pokemon", $"{Id}.png");
            return new Uri(filePath).AbsoluteUri;
        }
    }
    public string TypesDisplay => Types.Any(t => !string.IsNullOrEmpty(t.Name))
        ? string.Join(", ", Types
            .Where(t => !string.IsNullOrEmpty(t.Name))
            .Select(t => char.ToUpper(t.Name[0]) + t.Name[1..]))
        : string.Empty;

    /// <summary>
    /// Gets the abilities as a display string.
    /// </summary>
    public string AbilitiesDisplay
    {
        get
        {
            var abilities = new List<string>();
            if (!string.IsNullOrWhiteSpace(Ability1))
            {
                abilities.Add(Ability1);
            }
            if (!string.IsNullOrWhiteSpace(Ability2))
            {
                abilities.Add(Ability2);
            }
            if (!string.IsNullOrWhiteSpace(HiddenAbility))
            {
                abilities.Add($"{HiddenAbility} (hidden)");
            }
            return abilities.Count > 0 ? string.Join("\n", abilities) : string.Empty;
        }
    }
}
