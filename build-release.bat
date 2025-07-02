@echo off
setlocal enabledelayedexpansion

REM Info and error functions
set "INFO=[INFO] - "
set "ERRO=[ERRO] - "

echo %INFO% Versions:
dotnet --info
godot --version --quit

set "CURRENT_PATH=%cd%"

set "EXPORT_GUI=%CURRENT_PATH%\build\windows\GUI"
set "BIN_OUTPUT=%CURRENT_PATH%\build\export"

echo %INFO% Preparing build directory
rmdir /s /q build
mkdir "%EXPORT_GUI%"
mkdir "%BIN_OUTPUT%"

echo %INFO% Building export for Linux
godot --path LibreAutoTile.GUI --verbose --headless --export-release "Windows Desktop" "%EXPORT_GUI%\LibreAutoTile.GUI.exe"
if errorlevel 1 (
    echo %ERRO% Godot export failed
    exit /b 1
)

echo %INFO% Packaging exports
tar -cvf "%BIN_OUTPUT%\linux-gui.tar" -C "%CURRENT_PATH%\build\linux" GUI > nul

cd "%BIN_OUTPUT%"
certutil -hashfile linux-gui.tar SHA256 > linux-gui.tar.sha256

echo %INFO% Output in: %BIN_OUTPUT%