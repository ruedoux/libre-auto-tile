#!/bin/bash

set -e
shopt -s globstar

info() {
  echo "[INFO] - $@"
}

error() {
  echo "[ERRO] - $@"
}

build_gui() {
  info "Building GUI"
  info "Godot Version"
  godot --version --quit

  info "Download rcedit"
  if [ ! -f "rcedit-x64.exe" ]; then
      wget -O rcedit-x64.exe https://github.com/electron/rcedit/releases/download/v2.0.0/rcedit-x64.exe
  fi

  mkdir -p $GUI_LINUX_TEMP
  mkdir -p $GUI_WINDOWS_TEMP

  info "Building GUI export for Linux"
  if ! godot --path LibreAutoTile.GUI --verbose --headless --export-release "Linux" $GUI_LINUX_TEMP/LibreAutoTile.GUI.exe; then
      error "Godot linux export failed"
      exit 1
  fi

  info "Building GUI export for Windows"
  if ! godot --path LibreAutoTile.GUI --verbose --headless --export-release "Windows Desktop" $GUI_WINDOWS_TEMP/LibreAutoTile.GUI.exe; then
      error "Godot windows export failed"
      exit 1
  fi

  tar -czvf $EXPORT_OUTPUT/linux-gui.tar -C "$(dirname "$GUI_LINUX_TEMP")" GUI > /dev/null
  tar -czvf $EXPORT_OUTPUT/windows-gui.tar -C "$(dirname "$GUI_WINDOWS_TEMP")" GUI > /dev/null
}

build_libs() {
  info "Building Libraries"
  info "Dotnet Version"
  dotnet --info

  mkdir -p $LIB_TEMP

  info "Building libraries"
  rm -rf LibreAutoTile/bin
  rm -rf LibreAutoTile.GodotBindings/bin

  dotnet pack LibreAutoTile -c Release
  dotnet pack LibreAutoTile.GodotBindings -c Release

  cp LibreAutoTile/bin/Release/**/*.{dll,pdb,json} $LIB_TEMP/ 2>/dev/null || true
  cp LibreAutoTile.GodotBindings/bin/Release/**/*.{dll,pdb,json} $LIB_TEMP/ 2>/dev/null || true
  cp LibreAutoTile/bin/Release/*.nupkg $EXPORT_OUTPUT/
  cp LibreAutoTile.GodotBindings/bin/Release/*.nupkg $EXPORT_OUTPUT/

  tar -czvf $EXPORT_OUTPUT/libs.tar -C "$LIB_TEMP" . > /dev/null
}

CURRENT_PATH=$(pwd)
GUI_LINUX_TEMP="$CURRENT_PATH/build/linux/GUI"
GUI_WINDOWS_TEMP="$CURRENT_PATH/build/windows/GUI"
LIB_TEMP="$CURRENT_PATH/build/libs"
EXPORT_OUTPUT="$CURRENT_PATH/build/export"

info "Preparing build directory"
rm -rf ./build
mkdir -p $EXPORT_OUTPUT

case "$1" in
    --build-gui)
        build_gui
        ;;
    --build-libs)
        build_libs
        ;;
    --all)
        build_libs
        build_gui
        ;;
    *)
        echo "Usage: $0 [--build-gui|--build-libs|--all]"
        exit 1
        ;;
esac

info "Exports directory: $EXPORT_OUTPUT"