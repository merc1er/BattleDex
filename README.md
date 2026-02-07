# PokéBattleDex

A fast, native, modern Pokédex for Windows.

![Windows](https://img.shields.io/badge/Windows-0078D4?logo=windows&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![WinUI 3](https://img.shields.io/badge/WinUI-3-blue)

## Features

- 🔍 **Instant search** — Find any Pokémon by name
- 🎨 **Official sprites** — All 1025 Pokémon with artwork
- 🌐 **Multilingual** — English and French names
- 🌗 **Dark & Light themes** — Follows your Windows theme
- 🖥️ **Native look** — Built with WinUI 3
- ⚡ **Fast performance** — Lightweight and responsive

![PokéBattleDex](/.github/images/thumbnail.png)

## Installation

### Requirements

- Windows 10 or Windows 11
- [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)

### Quick Start

```powershell
git clone https://github.com/merc1er/PokeBattleDex.git
cd PokeBattleDex
dotnet build PokeBattleDex/PokeBattleDex.sln
dotnet run --project PokeBattleDex/PokeBattleDex.csproj
```

## Project Structure

```
PokeBattleDex/          # WinUI 3 application
PokeBattleDex.Core/     # Core library (models, data, services)
PokeBattleDex.Tests/    # Unit tests
```

## License

TBD
