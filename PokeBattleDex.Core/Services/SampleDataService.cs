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
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "bulbasaur",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "grass" },
                            new PokemonType { Name = "poison" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "ivysaur",
                Id = 2,
                NameEnglish = "Ivysaur",
                NameFrench = "Herbizarre",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "ivysaur",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "grass" },
                            new PokemonType { Name = "poison" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "venusaur",
                Id = 3,
                NameEnglish = "Venusaur",
                NameFrench = "Florizarre",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "venusaur",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "grass" },
                            new PokemonType { Name = "poison" }
                        }
                    },
                    new Pokemon
                    {
                        Name = "venusaur-mega",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "grass" },
                            new PokemonType { Name = "poison" }
                        }
                    },
                    new Pokemon
                    {
                        Name = "venusaur-gmax",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "grass" },
                            new PokemonType { Name = "poison" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "charmander",
                Id = 4,
                NameEnglish = "Charmander",
                NameFrench = "Salamèche",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "charmander",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "charmeleon",
                Id = 5,
                NameEnglish = "Charmeleon",
                NameFrench = "Reptincel",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "charmeleon",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "charizard",
                Id = 6,
                NameEnglish = "Charizard",
                NameFrench = "Dracaufeu",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "charizard",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" },
                            new PokemonType { Name = "flying" }
                        }
                    },
                    new Pokemon
                    {
                        Name = "charizard-mega-x",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" },
                            new PokemonType { Name = "dragon" }
                        }
                    },
                    new Pokemon
                    {
                        Name = "charizard-mega-y",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" },
                            new PokemonType { Name = "flying" }
                        }
                    },
                    new Pokemon
                    {
                        Name = "charizard-gmax",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "fire" },
                            new PokemonType { Name = "flying" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "squirtle",
                Id = 7,
                NameEnglish = "Squirtle",
                NameFrench = "Carapuce",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "squirtle",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "water" }
                        }
                    }
                }
            },
            new PokemonSpecies
            {
                Name = "wartortle",
                Id = 8,
                NameEnglish = "Wartortle",
                NameFrench = "Carabaffe",
                Pokemons = new List<Pokemon>
                {
                    new Pokemon
                    {
                        Name = "wartortle",
                        Types = new List<PokemonType>
                        {
                            new PokemonType { Name = "water" }
                        }
                    }
                }
            }
        };
    }
}
