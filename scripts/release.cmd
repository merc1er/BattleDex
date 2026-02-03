@echo off
setlocal

cd /d "%~dp0..\PokeBattleDex"

echo ===================================
echo Building PokeBattleDex for Release
echo ===================================

REM Clean previous builds
echo.
echo Cleaning previous builds...
dotnet clean -c Release

REM Build for x64
echo.
echo Building for x64...
dotnet publish -c Release -p:Platform=x64

if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed!
    exit /b 1
)

echo.
echo ===================================
echo Build completed successfully!
echo ===================================
echo.
echo Output: bin\Release\net9.0-windows10.0.19041.0\win-x64\publish\

endlocal
