@echo off
dotnet build .\PokeBattleDex\PokeBattleDex.csproj -p:Platform=x64
if %errorlevel% equ 0 (
    start "" ".\PokeBattleDex\bin\x64\Debug\net9.0-windows10.0.19041.0\win-x64\PokeBattleDex.exe"
)
