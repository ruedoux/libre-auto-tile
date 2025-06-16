@echo off
setlocal enabledelayedexpansion

:: Parse arguments
set GODOT_EXEC_PATH=
for %%A in (%*) do (
    set "arg=%%A"
    if "!arg:~0,13!"=="--godot-path=" (
        set "GODOT_EXEC_PATH=!arg:~13!"
    )
)

set "CURRENT_PATH=%cd%"
set "LINUX_EXPORT_GUI=%CURRENT_PATH%\build\linux\GUI"
set "WINDOWS_EXPORT_GUI=%CURRENT_PATH%\build\windows\GUI"
set "BIN_OUTPUT=%CURRENT_PATH%\build\export"

call :info Preparing build directory
rmdir /s /q build 2>nul
mkdir "%LINUX_EXPORT_GUI%"
mkdir "%WINDOWS_EXPORT_GUI%"
mkdir "%BIN_OUTPUT%"

call :info Building export for Linux
"%GODOT_EXEC_PATH%" --path LibreAutoTile.GUI --headless --export-release "Linux" "%LINUX_EXPORT_GUI%\LibreAutoTile.GUI.exe" >nul

call :info Building export for Windows Desktop
"%GODOT_EXEC_PATH%" --path LibreAutoTile.GUI --headless --export-release "Windows Desktop" "%WINDOWS_EXPORT_GUI%\LibreAutoTile.GUI.exe" >nul

call :info Packaging exports
tar -cvf "%BIN_OUTPUT%\linux-gui.tar" -C "%LINUX_EXPORT_GUI%\.." GUI >nul
tar -cvf "%BIN_OUTPUT%\windows-gui.tar" -C "%WINDOWS_EXPORT_GUI%\.." GUI >nul

cd /d "%BIN_OUTPUT%"
certutil -hashfile linux-gui.tar SHA256 > linux-gui.tar.sha256
certutil -hashfile windows-gui.tar SHA256 > windows-gui.tar.sha256

call :info Output in: %BIN_OUTPUT%
goto :eof

:info
echo [INFO] - %*
goto :eof