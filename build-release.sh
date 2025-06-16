#!/bin/bash

set -e

info() {
  echo "[INFO] - $@"
}

error() {
  echo "[ERRO] - $@"
}

for arg in "$@"; do
    case $arg in
        --godot-path=*)
        GODOT_EXEC_PATH="${arg#*=}"
        shift
        ;;
    esac
done

CURRENT_PATH=$(pwd)

LINUX_EXPORT_GUI="$CURRENT_PATH/build/linux/GUI"
WINDOWS_EXPORT_GUI="$CURRENT_PATH/build/windows/GUI"
BIN_OUTPUT="$CURRENT_PATH/build/export"

info "Preparing build directory"
rm -rf ./build
mkdir -p $LINUX_EXPORT_GUI
mkdir -p $WINDOWS_EXPORT_GUI
mkdir -p $BIN_OUTPUT

info "Building export for Linux"
$GODOT_EXEC_PATH --path LibreAutoTile.GUI --headless --export-release "Linux" $LINUX_EXPORT_GUI/LibreAutoTile.GUI.exe > /dev/null
info "Building export for Windows Desktop"
$GODOT_EXEC_PATH --path LibreAutoTile.GUI --headless --export-release "Windows Desktop" $WINDOWS_EXPORT_GUI/LibreAutoTile.GUI.exe > /dev/null

info "Packaging exports"
tar -cvf $BIN_OUTPUT/linux-gui.tar -C "$(dirname "$LINUX_EXPORT_GUI")" GUI > /dev/null
tar -cvf $BIN_OUTPUT/windows-gui.tar -C "$(dirname "$WINDOWS_EXPORT_GUI")" GUI > /dev/null

cd $BIN_OUTPUT
sha256sum linux-gui.tar > linux-gui.tar.sha256
sha256sum windows-gui.tar > windows-gui.tar.sha256
info "Output in: $BIN_OUTPUT"