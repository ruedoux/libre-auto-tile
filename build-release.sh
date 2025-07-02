#!/bin/bash

set -e

info() {
  echo "[INFO] - $@"
}

error() {
  echo "[ERRO] - $@"
}

info "Versions:"
dotnet --info
godot --version --quit

CURRENT_PATH=$(pwd)

EXPORT_GUI="$CURRENT_PATH/build/linux/GUI"
BIN_OUTPUT="$CURRENT_PATH/build/export"

info "Preparing build directory"
rm -rf ./build
mkdir -p $EXPORT_GUI
mkdir -p $BIN_OUTPUT

info "Building export for Linux"
if ! godot --path LibreAutoTile.GUI --verbose --headless --export-release "Linux" $EXPORT_GUI/LibreAutoTile.GUI.exe; then
    error "Godot export failed"
    exit 1
fi

info "Packaging exports"
tar -cvf $BIN_OUTPUT/linux-gui.tar -C "$(dirname "$EXPORT_GUI")" GUI > /dev/null

cd $BIN_OUTPUT
sha256sum linux-gui.tar > linux-gui.tar.sha256
info "Output in: $BIN_OUTPUT"