#nullable enable

using System.Reflection;
using System.Text.Json;
using BattleDex.Core.Contracts.Services;
using BattleDex.Core.Models;

namespace BattleDex.Core.Services;

/// <summary>
/// Service that provides Pokémon data loaded from an embedded CSV resource.
/// </summary>
public class SampleDataService : ISampleDataService
{
    private List<PokemonSpecies> _allPokemon = new();
    private IReadOnlyDictionary<int, IReadOnlyList<int>>? _regionalDex;

    public async Task<IEnumerable<PokemonSpecies>> GetPokemonDataAsync()
    {
        if (_allPokemon.Count == 0)
        {
            _allPokemon = new List<PokemonSpecies>(LoadPokemonFromCsv());
        }

        await Task.CompletedTask;
        return _allPokemon;
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetRegionalDexAsync()
    {
        _regionalDex ??= LoadRegionalDex();
        await Task.CompletedTask;
        return _regionalDex;
    }

    private static IReadOnlyDictionary<int, IReadOnlyList<int>> LoadRegionalDex()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "BattleDex.Core.Data.regional-dex.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");

        var raw = JsonSerializer.Deserialize<Dictionary<string, int[]>>(stream)
            ?? throw new InvalidOperationException($"Failed to parse {resourceName}");

        var result = new Dictionary<int, IReadOnlyList<int>>(raw.Count);
        foreach (var (key, ids) in raw)
        {
            result[int.Parse(key)] = ids;
        }
        return result;
    }

    private static IEnumerable<PokemonSpecies> LoadPokemonFromCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "BattleDex.Core.Data.pokemon.csv";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);

        // Read and parse header line to create column index map
        var headerLine = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            throw new InvalidOperationException("CSV file is empty or missing header.");
        }

        var headers = ParseCsvLine(headerLine);
        var columnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Length; i++)
        {
            columnIndex[headers[i]] = i;
        }

        var pokemon = new List<PokemonSpecies>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);
            if (fields.Length < headers.Length)
            {
                continue;
            }

            string GetField(string columnName) => columnIndex.TryGetValue(columnName, out var idx) && idx < fields.Length ? fields[idx] : string.Empty;

            var name = GetField("Name");
            var species = new PokemonSpecies
            {
                Id = int.Parse(GetField("#")),
                Name = name.ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace("'", ""),
                NameEnglish = name,
                NameFrench = GetField("FrenchName"),
                Types = ParseTypes(GetField("Type 1"), GetField("Type 2")),
                Total = int.Parse(GetField("Total")),
                HP = int.Parse(GetField("HP")),
                Attack = int.Parse(GetField("Attack")),
                Defense = int.Parse(GetField("Defense")),
                SpAtk = int.Parse(GetField("Sp. Atk")),
                SpDef = int.Parse(GetField("Sp. Def")),
                Speed = int.Parse(GetField("Speed")),
                Generation = int.Parse(GetField("Generation")),
                IsLegendary = GetField("Legendary").Equals("True", StringComparison.OrdinalIgnoreCase),
                Ability1 = GetField("Ability 1"),
                Ability2 = GetField("Ability 2"),
                HiddenAbility = GetField("Hidden Ability"),
                EvHP = int.TryParse(GetField("EV HP"), out var evHp) ? evHp : 0,
                EvAttack = int.TryParse(GetField("EV Attack"), out var evAtk) ? evAtk : 0,
                EvDefense = int.TryParse(GetField("EV Defense"), out var evDef) ? evDef : 0,
                EvSpAtk = int.TryParse(GetField("EV Sp. Atk"), out var evSpAtk) ? evSpAtk : 0,
                EvSpDef = int.TryParse(GetField("EV Sp. Def"), out var evSpDef) ? evSpDef : 0,
                EvSpeed = int.TryParse(GetField("EV Speed"), out var evSpd) ? evSpd : 0
            };

            pokemon.Add(species);
        }

        return pokemon;
    }

    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = "";
        var inQuotes = false;

        foreach (var c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        fields.Add(currentField);
        return fields.ToArray();
    }

    private static List<PokemonType> ParseTypes(string type1, string type2)
    {
        var types = new List<PokemonType>();

        if (!string.IsNullOrWhiteSpace(type1) && Enum.TryParse<PokemonType>(type1, true, out var parsedType1))
        {
            types.Add(parsedType1);
        }

        if (!string.IsNullOrWhiteSpace(type2) && Enum.TryParse<PokemonType>(type2, true, out var parsedType2))
        {
            types.Add(parsedType2);
        }

        return types;
    }
}
