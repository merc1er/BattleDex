namespace PokeBattleDex.Core.Models;

/// <summary>
/// Provides the Gen VI+ type effectiveness chart.
/// All multipliers are from the attacking type's perspective against a defending type.
/// </summary>
public static class TypeEffectiveness
{
    // Indexed by [attacker, defender] using PokemonType enum ordinals.
    // Row = attacking type, Column = defending type.
    private static readonly float[,] Chart = new float[18, 18]
    {
        //                    NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GND  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
        /* Normal   */ {      1f, 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f, .5f,  0f,  1f,  1f, .5f,  1f },
        /* Fire     */ {      1f, .5f, .5f, 1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f, .5f,  1f, .5f,  1f,  2f,  1f },
        /* Water    */ {      1f,  2f, .5f, 1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f, .5f,  1f,  1f,  1f },
        /* Electric */ {      1f,  1f,  2f, .5f,.5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f, .5f,  1f,  1f,  1f },
        /* Grass    */ {      1f, .5f,  2f, 1f, .5f,  1f,  1f, .5f,  2f, .5f,  1f, .5f,  2f,  1f, .5f,  1f, .5f,  1f },
        /* Ice      */ {      1f, .5f, .5f, 1f,  2f, .5f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  1f },
        /* Fighting */ {      2f,  1f,  1f, 1f,  1f,  2f,  1f, .5f,  1f, .5f, .5f, .5f,  2f,  0f,  1f,  2f,  2f, .5f },
        /* Poison   */ {      1f,  1f,  1f, 1f,  2f,  1f,  1f, .5f, .5f,  1f,  1f,  1f, .5f, .5f,  1f,  1f,  0f,  2f },
        /* Ground   */ {      1f,  2f,  1f, 2f, .5f,  1f,  1f,  2f,  1f,  0f,  1f, .5f,  2f,  1f,  1f,  1f,  2f,  1f },
        /* Flying   */ {      1f,  1f,  1f, .5f, 2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f, .5f,  1f,  1f,  1f, .5f,  1f },
        /* Psychic  */ {      1f,  1f,  1f, 1f,  1f,  1f,  2f,  2f,  1f,  1f, .5f,  1f,  1f,  1f,  1f,  0f, .5f,  1f },
        /* Bug      */ {      1f, .5f,  1f, 1f,  2f,  1f, .5f, .5f,  1f, .5f,  2f,  1f,  1f, .5f,  1f,  2f, .5f, .5f },
        /* Rock     */ {      1f,  2f,  1f, 1f,  1f,  2f, .5f,  1f, .5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f, .5f,  1f },
        /* Ghost    */ {      0f,  1f,  1f, 1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f,  1f,  1f },
        /* Dragon   */ {      1f,  1f,  1f, 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f, .5f,  0f },
        /* Dark     */ {      1f,  1f,  1f, 1f,  1f,  1f, .5f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f, .5f,  1f, .5f },
        /* Steel    */ {      1f, .5f, .5f,.5f,  1f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f, .5f,  2f },
        /* Fairy    */ {      1f, .5f,  1f, 1f,  1f,  1f,  2f, .5f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f, .5f,  1f },
    };

    /// <summary>
    /// Gets the effectiveness multiplier when <paramref name="attacker"/> attacks <paramref name="defender"/>.
    /// </summary>
    public static float GetMultiplier(PokemonType attacker, PokemonType defender)
        => Chart[(int)attacker, (int)defender];

    /// <summary>
    /// Calculates the combined defensive multiplier for a Pokémon with the given types
    /// when attacked by <paramref name="attackingType"/>.
    /// For dual types the multipliers are multiplied together.
    /// </summary>
    public static float GetDefensiveMultiplier(PokemonType attackingType, IReadOnlyList<PokemonType> defendingTypes)
    {
        var multiplier = 1f;
        foreach (var defType in defendingTypes)
        {
            multiplier *= GetMultiplier(attackingType, defType);
        }
        return multiplier;
    }

    /// <summary>
    /// Returns all attacking types grouped by their effectiveness against the given defending types.
    /// </summary>
    public static TypeMatchup GetDefensiveMatchup(IReadOnlyList<PokemonType> defendingTypes)
    {
        var weaknesses = new List<TypeMultiplier>();
        var resistances = new List<TypeMultiplier>();
        var immunities = new List<PokemonType>();

        foreach (PokemonType atkType in Enum.GetValues(typeof(PokemonType)))
        {
            var mult = GetDefensiveMultiplier(atkType, defendingTypes);

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
