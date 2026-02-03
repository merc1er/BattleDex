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

        // Skip header line
        reader.ReadLine();

        var pokemon = new List<PokemonSpecies>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);
            if (fields.Length < 14)
            {
                continue;
            }

            // CSV columns: #,Name,Type 1,Type 2,Total,HP,Attack,Defense,Sp. Atk,Sp. Def,Speed,Generation,Legendary,FrenchName
            var species = new PokemonSpecies
            {
                Id = int.Parse(fields[0]),
                Name = fields[1].ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace("'", ""),
                NameEnglish = fields[1],
                NameFrench = fields[13],
                Types = ParseTypes(fields[2], fields[3]),
                Total = int.Parse(fields[4]),
                HP = int.Parse(fields[5]),
                Attack = int.Parse(fields[6]),
                Defense = int.Parse(fields[7]),
                SpAtk = int.Parse(fields[8]),
                SpDef = int.Parse(fields[9]),
                Speed = int.Parse(fields[10]),
                Generation = int.Parse(fields[11]),
                IsLegendary = fields[12].Equals("True", StringComparison.OrdinalIgnoreCase)
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

        if (!string.IsNullOrWhiteSpace(type1))
        {
            types.Add(new PokemonType { Name = type1.ToLowerInvariant() });
        }

        if (!string.IsNullOrWhiteSpace(type2))
        {
            types.Add(new PokemonType { Name = type2.ToLowerInvariant() });
        }

        return types;
    }
}
