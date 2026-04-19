namespace BattleDex.Core.Models;

/// <summary>
/// Identifies which generation's type chart to use.
/// </summary>
public enum GenerationChart
{
    Gen1 = 1,
    Gen2 = 2,
    Gen3 = 3,
    Gen4 = 4,
    Gen5 = 5,
    Gen6 = 6,
    Gen7 = 7,
    Gen8 = 8,
    Gen9 = 9,
}

/// <summary>
/// Provides type effectiveness charts for Gen II–V and Gen VI+.
/// All multipliers are from the attacking type's perspective against a defending type.
/// </summary>
public static class TypeEffectiveness
{
    /// <summary>Returns the number of types that exist in the given generation.</summary>
    private static int GetTypeCount(GenerationChart gen) => gen switch
    {
        GenerationChart.Gen1 => 15, // Normal..Dragon (no Dark/Steel/Fairy)
        GenerationChart.Gen2 or GenerationChart.Gen3 or GenerationChart.Gen4 or GenerationChart.Gen5 => 17, // Normal..Steel (no Fairy)
        _ => 18, // Normal..Fairy (Gen VI+)
    };

    // Gen VI+ chart: 18×18 (includes Fairy).
    private static readonly float[,] Gen6PlusChart = new float[18, 18]
    {
        //               NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GND  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
        /* Normal   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f, .5f,  0f,  1f,  1f, .5f,  1f },
        /* Fire     */ { 1f, .5f, .5f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  1f,  2f,  1f },
        /* Water    */ { 1f,  2f, .5f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f, .5f,  1f,  1f,  1f },
        /* Electric */ { 1f,  1f,  2f, .5f, .5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f, .5f,  1f,  1f,  1f },
        /* Grass    */ { 1f, .5f,  2f,  1f, .5f,  1f,  1f, .5f,  2f, .5f,  1f, .5f,  2f,  1f, .5f,  1f, .5f,  1f },
        /* Ice      */ { 1f, .5f, .5f,  1f,  2f, .5f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  1f },
        /* Fighting */ { 2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  1f, .5f, .5f, .5f,  2f,  0f,  1f,  2f,  2f, .5f },
        /* Poison   */ { 1f,  1f,  1f,  1f,  2f,  1f,  1f, .5f, .5f,  1f,  1f,  1f, .5f, .5f,  1f,  1f,  0f,  2f },
        /* Ground   */ { 1f,  2f,  1f,  2f, .5f,  1f,  1f,  2f,  1f,  0f,  1f, .5f,  2f,  1f,  1f,  1f,  2f,  1f },
        /* Flying   */ { 1f,  1f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f, .5f,  1f,  1f,  1f, .5f,  1f },
        /* Psychic  */ { 1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f, .5f,  1f,  1f,  1f,  1f,  0f, .5f,  1f },
        /* Bug      */ { 1f, .5f,  1f,  1f,  2f,  1f, .5f, .5f,  1f, .5f,  2f,  1f,  1f, .5f,  1f,  2f, .5f, .5f },
        /* Rock     */ { 1f,  2f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f, .5f,  1f },
        /* Ghost    */ { 0f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f,  1f,  1f },
        /* Dragon   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  0f },
        /* Dark     */ { 1f,  1f,  1f,  1f,  1f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f,  1f, .5f },
        /* Steel    */ { 1f, .5f, .5f, .5f,  1f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f, .5f,  2f },
        /* Fairy    */ { 1f, .5f,  1f,  1f,  1f,  1f,  2f, .5f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f, .5f,  1f },
    };

    // Gen II–V chart: 17×17 (no Fairy; Steel resists Ghost & Dark).
    private static readonly float[,] Gen2To5Chart = new float[17, 17]
    {
        //               NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GND  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE
        /* Normal   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f, .5f,  0f,  1f,  1f, .5f },
        /* Fire     */ { 1f, .5f, .5f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  1f,  2f },
        /* Water    */ { 1f,  2f, .5f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f, .5f,  1f,  1f },
        /* Electric */ { 1f,  1f,  2f, .5f, .5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f, .5f,  1f,  1f },
        /* Grass    */ { 1f, .5f,  2f,  1f, .5f,  1f,  1f, .5f,  2f, .5f,  1f, .5f,  2f,  1f, .5f,  1f, .5f },
        /* Ice      */ { 1f, .5f, .5f,  1f,  2f, .5f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f },
        /* Fighting */ { 2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  1f, .5f, .5f, .5f,  2f,  0f,  1f,  2f,  2f },
        /* Poison   */ { 1f,  1f,  1f,  1f,  2f,  1f,  1f, .5f, .5f,  1f,  1f,  1f, .5f, .5f,  1f,  1f,  0f },
        /* Ground   */ { 1f,  2f,  1f,  2f, .5f,  1f,  1f,  2f,  1f,  0f,  1f, .5f,  2f,  1f,  1f,  1f,  2f },
        /* Flying   */ { 1f,  1f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f, .5f,  1f,  1f,  1f, .5f },
        /* Psychic  */ { 1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f, .5f,  1f,  1f,  1f,  1f,  0f, .5f },
        /* Bug      */ { 1f, .5f,  1f,  1f,  2f,  1f, .5f, .5f,  1f, .5f,  2f,  1f,  1f, .5f,  1f,  2f, .5f },
        /* Rock     */ { 1f,  2f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f, .5f },
        /* Ghost    */ { 0f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f, .5f },
        /* Dragon   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f, .5f },
        /* Dark     */ { 1f,  1f,  1f,  1f,  1f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f, .5f },
        /* Steel    */ { 1f, .5f, .5f, .5f,  1f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f, .5f },
    };

    // Gen I chart: 15×15 (no Dark/Steel/Fairy).
    // Differences from Gen II+:
    //   • Bug → Poison: 2× (was the only way to hit Bug hard; changed to 0.5× in Gen II)
    //   • Poison → Bug: 2× (changed to 1× in Gen II)
    //   • Ghost → Psychic: 0× (a well-known bug; changed to 2× in Gen II)
    private static readonly float[,] Gen1Chart = new float[15, 15]
    {
        //               NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GND  FLY  PSY  BUG  ROC  GHO  DRA
        /* Normal   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f, .5f,  0f,  1f },
        /* Fire     */ { 1f, .5f, .5f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f, .5f,  1f, .5f },
        /* Water    */ { 1f,  2f, .5f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f, .5f },
        /* Electric */ { 1f,  1f,  2f, .5f, .5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f, .5f },
        /* Grass    */ { 1f, .5f,  2f,  1f, .5f,  1f,  1f, .5f,  2f, .5f,  1f, .5f,  2f,  1f, .5f },
        /* Ice      */ { 1f, .5f, .5f,  1f,  2f, .5f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f },
        /* Fighting */ { 2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  1f, .5f, .5f, .5f,  2f,  0f,  1f },
        /* Poison   */ { 1f,  1f,  1f,  1f,  2f,  1f,  1f, .5f, .5f,  1f,  1f,  2f, .5f, .5f,  1f },
        /* Ground   */ { 1f,  2f,  1f,  2f, .5f,  1f,  1f,  2f,  1f,  0f,  1f, .5f,  2f,  1f,  1f },
        /* Flying   */ { 1f,  1f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f, .5f,  1f,  1f },
        /* Psychic  */ { 1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f, .5f,  1f,  1f,  1f,  1f },
        /* Bug      */ { 1f, .5f,  1f,  1f,  2f,  1f, .5f,  2f,  1f, .5f,  2f,  1f,  1f, .5f,  1f },
        /* Rock     */ { 1f,  2f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f },
        /* Ghost    */ { 0f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  0f,  1f,  1f,  2f,  1f },
        /* Dragon   */ { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f },
    };

    private static float[,] GetChart(GenerationChart gen) => gen switch
    {
        GenerationChart.Gen1 => Gen1Chart,
        GenerationChart.Gen2 or GenerationChart.Gen3 or GenerationChart.Gen4 or GenerationChart.Gen5 => Gen2To5Chart,
        _ => Gen6PlusChart,
    };

    /// <summary>
    /// Gets the effectiveness multiplier when <paramref name="attacker"/> attacks <paramref name="defender"/>.
    /// </summary>
    public static float GetMultiplier(PokemonType attacker, PokemonType defender, GenerationChart gen = GenerationChart.Gen9)
    {
        var chart = GetChart(gen);
        var atkIdx = (int)attacker;
        var defIdx = (int)defender;

        // Type doesn't exist in the chosen chart — treat as neutral.
        if (atkIdx >= chart.GetLength(0) || defIdx >= chart.GetLength(1))
        {
            return 1f;
        }

        return chart[atkIdx, defIdx];
    }

    /// <summary>
    /// Calculates the combined defensive multiplier for a Pokémon with the given types
    /// when attacked by <paramref name="attackingType"/>.
    /// For dual types the multipliers are multiplied together.
    /// </summary>
    public static float GetDefensiveMultiplier(PokemonType attackingType, IReadOnlyList<PokemonType> defendingTypes, GenerationChart gen = GenerationChart.Gen9)
    {
        var multiplier = 1f;
        foreach (var defType in defendingTypes)
        {
            multiplier *= GetMultiplier(attackingType, defType, gen);
        }
        return multiplier;
    }

    /// <summary>
    /// Returns all attacking types grouped by their effectiveness against the given defending types.
    /// </summary>
    public static TypeMatchup GetDefensiveMatchup(IReadOnlyList<PokemonType> defendingTypes, GenerationChart gen = GenerationChart.Gen9)
    {
        var typeCount = GetTypeCount(gen);
        var weaknesses = new List<TypeMultiplier>();
        var resistances = new List<TypeMultiplier>();
        var immunities = new List<PokemonType>();

        foreach (PokemonType atkType in Enum.GetValues(typeof(PokemonType)))
        {
            if ((int)atkType >= typeCount)
            {
                continue;
            }

            var mult = GetDefensiveMultiplier(atkType, defendingTypes, gen);

            if (mult == 0f)
            {
                immunities.Add(atkType);
            }
            else if (mult > 1f)
            {
                weaknesses.Add(new TypeMultiplier(atkType, mult));
            }
            else if (mult < 1f)
            {
                resistances.Add(new TypeMultiplier(atkType, mult));
            }
        }

        return new TypeMatchup(weaknesses, resistances, immunities);
    }
}

/// <summary>
/// A type paired with its effectiveness multiplier.
/// </summary>
public record TypeMultiplier(PokemonType Type, float Multiplier)
{
    public string MultiplierDisplay => Multiplier switch
    {
        4f => "×4",
        2f => "×2",
        0.5f => "×½",
        0.25f => "×¼",
        _ => $"×{Multiplier}"
    };
}

/// <summary>
/// Contains the full defensive type matchup for a Pokémon.
/// </summary>
public record TypeMatchup(
    List<TypeMultiplier> Weaknesses,
    List<TypeMultiplier> Resistances,
    List<PokemonType> Immunities);
