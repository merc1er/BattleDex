#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BattleDex.Core.Models;

/// <summary>
/// Represents a Pokémon species.
/// </summary>
public class PokemonSpecies : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool _isCaught;

    /// <summary>
    /// Whether the user has marked this Pokémon as caught.
    /// </summary>
    public bool IsCaught
    {
        get => _isCaught;
        set
        {
            if (_isCaught != value)
            {
                _isCaught = value;
                OnPropertyChanged();
            }
        }
    }

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
    /// EV yield for HP.
    /// </summary>
    public int EvHP
    {
        get; set;
    }

    /// <summary>
    /// EV yield for Attack.
    /// </summary>
    public int EvAttack
    {
        get; set;
    }

    /// <summary>
    /// EV yield for Defense.
    /// </summary>
    public int EvDefense
    {
        get; set;
    }

    /// <summary>
    /// EV yield for Special Attack.
    /// </summary>
    public int EvSpAtk
    {
        get; set;
    }

    /// <summary>
    /// EV yield for Special Defense.
    /// </summary>
    public int EvSpDef
    {
        get; set;
    }

    /// <summary>
    /// EV yield for Speed.
    /// </summary>
    public int EvSpeed
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
    /// Gets the formatted Pokédex number (e.g., "#001").
    /// </summary>
    public string IdDisplay => $"#{Id:D3}";

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

    /// <summary>
    /// Gets the file path for the Pokémon's shiny sprite image.
    /// </summary>
    public string ShinySpriteUri
    {
        get
        {
            if (string.IsNullOrEmpty(SpriteBasePath))
            {
                return string.Empty;
            }
            var filePath = Path.Combine(SpriteBasePath, "Assets", "Sprites", "Pokemon", "Shiny", $"{Id}.png");
            return new Uri(filePath).AbsoluteUri;
        }
    }
    public string TypesDisplay => string.Join(", ", Types);

    /// <summary>
    /// Gets the Pokémon Database (pokemondb.net) page for this Pokémon.
    /// </summary>
    public Uri PokemonDbUri => new($"https://pokemondb.net/pokedex/{Name}");

    /// <summary>
    /// Gets the Bulbapedia page for this Pokémon.
    /// </summary>
    public Uri BulbapediaUri => new($"https://bulbapedia.bulbagarden.net/wiki/{Uri.EscapeDataString(GetBulbapediaTitle())}_(Pok%C3%A9mon)");

    /// <summary>
    /// Smogon's short code for each generation's dex, as used in its URLs.
    /// </summary>
    private static readonly Dictionary<GenerationChart, string> SmogonGenerationCodes = new()
    {
        [GenerationChart.Gen1] = "rb",
        [GenerationChart.Gen2] = "gs",
        [GenerationChart.Gen3] = "rs",
        [GenerationChart.Gen4] = "dp",
        [GenerationChart.Gen5] = "bw",
        [GenerationChart.Gen6] = "xy",
        [GenerationChart.Gen7] = "sm",
        [GenerationChart.Gen8] = "ss",
        [GenerationChart.Gen9] = "sv",
    };

    /// <summary>
    /// Gets the Smogon strategy dex page for this Pokémon in the given generation,
    /// or null if Smogon's dex for that generation doesn't have it.
    /// </summary>
    public Uri? GetSmogonUri(GenerationChart generation)
    {
        if (Generation > (int)generation)
        {
            return null;
        }

        // The Legends: Arceus Pokémon (Wyrdeer to Enamorus) are Gen VIII but aren't in Smogon's Sword/Shield dex.
        if (generation == GenerationChart.Gen8 && Id is >= 899 and <= 905)
        {
            return null;
        }

        // Smogon only lists Meowstic by gender form; link to the default (male) one.
        var slug = Id == 678 ? "meowstic-m" : Name;
        return new($"https://www.smogon.com/dex/{SmogonGenerationCodes[generation]}/pokemon/{slug}/");
    }

    /// <summary>
    /// Bulbapedia article titles that can't be derived from the species name.
    /// </summary>
    private static readonly Dictionary<int, string> BulbapediaTitleOverrides = new()
    {
        [29] = "Nidoran♀",
        [32] = "Nidoran♂",
        [83] = "Farfetch'd",
        [122] = "Mr._Mime",
        [250] = "Ho-Oh",
        [439] = "Mime_Jr.",
        [474] = "Porygon-Z",
        [669] = "Flabébé",
        [772] = "Type:_Null",
        [782] = "Jangmo-o",
        [783] = "Hakamo-o",
        [784] = "Kommo-o",
        [865] = "Sirfetch'd",
        [866] = "Mr._Rime",
        [1001] = "Wo-Chien",
        [1002] = "Chien-Pao",
        [1003] = "Ting-Lu",
        [1004] = "Chi-Yu",
    };

    private string GetBulbapediaTitle()
    {
        if (BulbapediaTitleOverrides.TryGetValue(Id, out var title))
        {
            return title;
        }

        // e.g. "great-tusk" -> "Great_Tusk"
        var parts = Name.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..]);
        return string.Join("_", parts);
    }

    /// <summary>
    /// Gets the defensive type matchup (weaknesses, resistances, immunities) for this Pokémon.
    /// </summary>
    public TypeMatchup DefensiveMatchup => TypeEffectiveness.GetDefensiveMatchup(Types);

    /// <summary>
    /// Per-generation EV yield overrides for Pokémon whose yields changed across generations.
    /// Only generations that differ from the default (Gen IX) values stored in the CSV are listed.
    /// </summary>
    private static readonly Dictionary<int, Dictionary<GenerationChart, (int HP, int Atk, int Def, int SpAtk, int SpDef, int Spd)>> EvOverrides = new()
    {
        [193] = new() // Yanma
        {
            [GenerationChart.Gen3] = (0, 0, 0, 0, 0, 2),
        },
        [199] = new() // Slowking
        {
            [GenerationChart.Gen3] = (0, 0, 0, 0, 3, 0),
            [GenerationChart.Gen4] = (0, 0, 0, 0, 3, 0),
            [GenerationChart.Gen5] = (0, 0, 0, 0, 3, 0),
            [GenerationChart.Gen6] = (0, 0, 0, 0, 3, 0),
            [GenerationChart.Gen7] = (0, 0, 0, 0, 3, 0),
        },
        [200] = new() // Misdreavus
        {
            [GenerationChart.Gen3] = (0, 0, 0, 1, 1, 0),
        },
        [242] = new() // Blissey
        {
            [GenerationChart.Gen3] = (2, 0, 0, 0, 0, 0),
        },
        [315] = new() // Roselia
        {
            [GenerationChart.Gen3] = (0, 0, 0, 1, 0, 0),
        },
        [355] = new() // Duskull
        {
            [GenerationChart.Gen3] = (0, 0, 1, 0, 1, 0),
        },
        [356] = new() // Dusclops
        {
            [GenerationChart.Gen3] = (0, 0, 1, 0, 2, 0),
            [GenerationChart.Gen4] = (0, 0, 0, 0, 1, 1),
        },
    };

    /// <summary>
    /// Gets the EV yield as a display string for the given generation.
    /// </summary>
    public string GetEvYieldDisplay(GenerationChart generation = GenerationChart.Gen9)
    {
        int hp = EvHP, atk = EvAttack, def = EvDefense, spAtk = EvSpAtk, spDef = EvSpDef, spd = EvSpeed;

        if (EvOverrides.TryGetValue(Id, out var genOverrides) && genOverrides.TryGetValue(generation, out var overrides))
        {
            (hp, atk, def, spAtk, spDef, spd) = overrides;
        }

        var yields = new List<string>();
        if (hp > 0) yields.Add($"{hp} HP");
        if (atk > 0) yields.Add($"{atk} Attack");
        if (def > 0) yields.Add($"{def} Defense");
        if (spAtk > 0) yields.Add($"{spAtk} Sp. Atk");
        if (spDef > 0) yields.Add($"{spDef} Sp. Def");
        if (spd > 0) yields.Add($"{spd} Speed");
        return yields.Count > 0 ? string.Join("\n", yields) : "None";
    }

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
