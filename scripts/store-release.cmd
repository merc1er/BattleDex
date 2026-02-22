@echo off
setlocal enabledelayedexpansion

cd /d "%~dp0..\PokeBattleDex"

echo =============================================
echo  PokeBattleDex - Microsoft Store MSIX Build
echo =============================================

REM --- Configuration ---
set "PLATFORMS=x64 x86 arm64"
set "CONFIG=Release"
set "CSPROJ=PokeBattleDex.csproj"
set "BUNDLE_DIR=bin\%CONFIG%\AppPackages"
set "STAGING_DIR=bin\%CONFIG%\BundleStaging"
set "MAKEAPPX="

REM --- Locate makeappx.exe from Windows SDK ---
echo.
echo Locating Windows SDK makeappx.exe...
for /f "delims=" %%F in ('where /r "C:\Program Files (x86)\Windows Kits\10\bin" makeappx.exe 2^>nul ^| findstr /i "x64\\makeappx.exe"') do (
    set "MAKEAPPX=%%F"
)
if "%MAKEAPPX%"=="" (
    echo ERROR: Could not find makeappx.exe. Install the Windows 10/11 SDK.
    exit /b 1
)
echo Found: %MAKEAPPX%

REM --- Read version from Package.appxmanifest ---
for /f "delims=" %%V in ('powershell -NoProfile -Command "([xml](Get-Content Package.appxmanifest)).Package.Identity.Version"') do set "VERSION=%%V"
if "%VERSION%"=="" set "VERSION=1.0.0.0"
echo Package version: %VERSION%

REM --- Clean previous output ---
echo.
echo Cleaning previous builds...
dotnet clean %CSPROJ% -c %CONFIG% >nul 2>&1
if exist "%BUNDLE_DIR%" rmdir /s /q "%BUNDLE_DIR%"
if exist "%STAGING_DIR%" rmdir /s /q "%STAGING_DIR%"
mkdir "%BUNDLE_DIR%" 2>nul
mkdir "%STAGING_DIR%" 2>nul

REM --- Build MSIX for each platform ---
for %%P in (%PLATFORMS%) do (
    echo.
    echo =============================================
    echo  Building %%P ...
    echo =============================================
    dotnet msbuild %CSPROJ% -restore ^
        -p:Configuration=%CONFIG% ^
        -p:Platform=%%P ^
        -p:UapAppxPackageBuildMode=StoreUpload ^
        -p:AppxBundle=Never ^
        -p:AppxPackageSigningEnabled=false ^
        -p:GenerateAppInstallerFile=false ^
        -p:GenerateAppxPackageOnBuild=true

    if !ERRORLEVEL! neq 0 (
        echo ERROR: %%P build failed!
        exit /b 1
    )
)

REM --- Gather MSIX files into staging directory ---
echo.
echo Collecting MSIX packages...
set "FOUND=0"
for /r "bin" %%M in (*_%CONFIG%_*.msix PokeBattleDex_*_x64.msix PokeBattleDex_*_x86.msix PokeBattleDex_*_arm64.msix) do (
    REM Only copy .msix files (not .msixbundle etc.)
    echo %%~xM | findstr /i ".msix" >nul && echo %%~xM | findstr /i "bundle upload" >nul || (
        copy /y "%%M" "%STAGING_DIR%\" >nul 2>&1
    )
)

REM Explicit copy as fallback
for %%P in (%PLATFORMS%) do (
    for /r "bin\%%P\%CONFIG%" %%M in (PokeBattleDex_*.msix) do (
        if /i "%%~xM"==".msix" (
            copy /y "%%M" "%STAGING_DIR%\" >nul
            set /a FOUND+=1
        )
    )
)

echo Found packages:
dir /b "%STAGING_DIR%\*.msix" 2>nul
if not exist "%STAGING_DIR%\*.msix" (
    echo ERROR: No MSIX packages found in staging directory!
    exit /b 1
)

REM --- Create MSIX Bundle ---
echo.
echo =============================================
echo  Creating MSIX Bundle...
echo =============================================
set "BUNDLE_PATH=%BUNDLE_DIR%\PokeBattleDex_%VERSION%.msixbundle"
"%MAKEAPPX%" bundle /d "%STAGING_DIR%" /p "%BUNDLE_PATH%" /o
if %ERRORLEVEL% neq 0 (
    echo ERROR: Bundle creation failed!
    exit /b 1
)

REM --- Create .msixupload (zip of the bundle) ---
echo.
echo Creating .msixupload for Store submission...
set "UPLOAD_PATH=%BUNDLE_DIR%\PokeBattleDex_%VERSION%.msixupload"
if exist "%UPLOAD_PATH%" del "%UPLOAD_PATH%"
powershell -NoProfile -Command "Compress-Archive -Path '%BUNDLE_PATH%' -DestinationPath '%UPLOAD_PATH%' -Force"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to create .msixupload!
    exit /b 1
)

REM --- Cleanup ---
rmdir /s /q "%STAGING_DIR%" 2>nul

REM --- Summary ---
echo.
echo =============================================
echo  Build Complete!
echo =============================================
echo.
echo  Bundle:  %BUNDLE_PATH%
echo  Upload:  %UPLOAD_PATH%
echo.
echo  Upload the .msixupload file to Microsoft
echo  Partner Center to submit to the Store.
echo =============================================

endlocal
