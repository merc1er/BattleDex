using PokeBattleDex.Core.Contracts.Services;
using PokeBattleDex.Core.Models;

namespace PokeBattleDex.Core.Services;

/// <summary>
/// Service that provides sample Pokémon data.
/// </summary>
public class SampleDataService : ISampleDataService
{
    private List<PokemonSpecies> _allPokemon = new();

    public async Task<IEnumerable<PokemonSpecies>> GetPokemonDataAsync()
    {
        if (_allPokemon.Count == 0)
        {
            _allPokemon = new List<PokemonSpecies>(AllPokemon());
        }

        await Task.CompletedTask;
        return _allPokemon;
    }

    private static IEnumerable<PokemonSpecies> AllPokemon()
    {
        return new List<PokemonSpecies>
        {
            new PokemonSpecies
            {
                Name = "bulbasaur",
                Id = 1,
                NameEnglish = "Bulbasaur",
                NameFrench = "Bulbizarre",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "grass" },
                    new PokemonType { Name = "poison" }
                }
            },
            new PokemonSpecies
            {
                Name = "ivysaur",
                Id = 2,
                NameEnglish = "Ivysaur",
                NameFrench = "Herbizarre",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "grass" },
                    new PokemonType { Name = "poison" }
                }
            },
            new PokemonSpecies
            {
                Name = "venusaur",
                Id = 3,
                NameEnglish = "Venusaur",
                NameFrench = "Florizarre",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "grass" },
                    new PokemonType { Name = "poison" }
                }
            },
            new PokemonSpecies
            {
                Name = "charmander",
                Id = 4,
                NameEnglish = "Charmander",
                NameFrench = "Salamèche",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "fire" }
                }
            },
            new PokemonSpecies
            {
                Name = "charmeleon",
                Id = 5,
                NameEnglish = "Charmeleon",
                NameFrench = "Reptincel",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "fire" }
                }
            },
            new PokemonSpecies
            {
                Name = "charizard",
                Id = 6,
                NameEnglish = "Charizard",
                NameFrench = "Dracaufeu",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "fire" },
                    new PokemonType { Name = "flying" }
                }
            },
            new PokemonSpecies
            {
                Name = "squirtle",
                Id = 7,
                NameEnglish = "Squirtle",
                NameFrench = "Carapuce",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "water" }
                }
            },
            new PokemonSpecies
            {
                Name = "wartortle",
                Id = 8,
                NameEnglish = "Wartortle",
                NameFrench = "Carabaffe",
                Types = new List<PokemonType>
                {
                    new PokemonType { Name = "water" }
                }
            }
        };
    }
}
