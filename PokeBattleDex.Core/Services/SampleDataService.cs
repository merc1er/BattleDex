using System.Reflection;
using PokeBattleDex.Core.Contracts.Services;
using PokeBattleDex.Core.Models;

namespace PokeBattleDex.Core.Services;

/// <summary>
/// Service that provides Pokémon data loaded from an embedded CSV resource.
/// </summary>
public class SampleDataService : ISampleDataService
{
    private List<PokemonSpecies> _allPokemon = new();

    public async Task<IEnumerable<PokemonSpecies>> GetPokemonDataAsync()
    {
        if (_allPokemon.Count == 0)
        {
            _allPokemon = new List<PokemonSpecies>(LoadPokemonFromCsv());
        }

        await Task.CompletedTask;
        return _allPokemon;
    }

    private static IEnumerable<PokemonSpecies> LoadPokemonFromCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "PokeBattleDex.Core.Data.pokemon.csv";

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
                HiddenAbility = GetField("Hidden Ability")
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
