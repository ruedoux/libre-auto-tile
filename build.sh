#!/bin/bash

set -euo pipefail
shopt -s globstar

RED_COLOR='\033[0;31m'
BLUE_COLOR='\033[0;34m'
NO_COLOR='\033[0m'

info() { echo -e "${BLUE_COLOR}[INFO]${NO_COLOR} $@"; }
error() { echo -e "${RED_COLOR}[ERROR]${NO_COLOR} $@"; }

build_gui() {
  info "Building GUI"
  info "Godot Version"
  godot --version

  info "Download rcedit"
  if [ ! -f "rcedit-x64.exe" ]; then
      wget -O rcedit-x64.exe https://github.com/electron/rcedit/releases/download/v2.0.0/rcedit-x64.exe
  fi

  mkdir -p "$GUI_LINUX_TEMP"
  mkdir -p "$GUI_WINDOWS_TEMP"

  info "Building GUI export for Linux"
  godot --path LibreAutoTile.GUI --verbose --headless --export-release "Linux" "$GUI_LINUX_TEMP"/LibreAutoTile.GUI
  tar -czvf "$EXPORT_OUTPUT"/linux-gui.tar.gz -C "$GUI_LINUX_TEMP" . > /dev/null

  info "Building GUI export for Windows"
  godot --path LibreAutoTile.GUI --verbose --headless --export-release "Windows Desktop" "$GUI_WINDOWS_TEMP"/LibreAutoTile.GUI.exe
  tar -czvf "$EXPORT_OUTPUT"/windows-gui.tar.gz -C "$GUI_WINDOWS_TEMP" . > /dev/null
}

build_libs() {
  info "Building Libraries"
  info "Dotnet Version"
  dotnet --info

  mkdir -p "$LIB_TEMP"

  info "Building libraries"
  rm -rf LibreAutoTile/bin
  rm -rf LibreAutoTile.GodotBindings/bin

  dotnet restore
  dotnet build LibreAutoTile -c Release
  dotnet build LibreAutoTile.GodotBindings -c Release

  find LibreAutoTile/bin/Release/ -type f \( -name "*.dll" -o -name "*.pdb" -o -name "*.json" \) -exec cp {} "$LIB_TEMP/" \;
  find LibreAutoTile.GodotBindings/bin/Release/ -type f \( -name "*.dll" -o -name "*.pdb" -o -name "*.json" \) -exec cp {} "$LIB_TEMP/" \;
  tar -czvf "$EXPORT_OUTPUT"/libs.tar.gz -C "$LIB_TEMP" . > /dev/null
}

run_benchmark() {
  info "Running benchmarks"
  (cd LibreAutoTile.Benchmarks && dotnet run -c Release --filter '*')
}

run_all_tests() {
  info "Running core library tests"
  if ! (cd LibreAutoTile.Tests && dotnet run); then
      error "Core library tests failed"
      exit 1
  fi

  info "Running Godot bindings tests"
  if ! (cd LibreAutoTile.GodotBindings.Tests && ./run-tests.sh); then
      error "Godot bindings tests failed"
      exit 1
  fi
}

help() {
  echo "Usage: $0 [--build-gui|--build-libs|--build-all|--run-tests|--run-benchmark]"
}

if [[ $# -eq 0 ]]; then
    help
    exit 1
fi

CURRENT_PATH=$(pwd)
GUI_LINUX_TEMP="$CURRENT_PATH/build/linux/GUI"
GUI_WINDOWS_TEMP="$CURRENT_PATH/build/windows/GUI"
LIB_TEMP="$CURRENT_PATH/build/libs"
EXPORT_OUTPUT="$CURRENT_PATH/build/export"

case "$1" in
  --build-gui)
    mkdir -p "$EXPORT_OUTPUT"
    build_gui
    ;;
  --build-libs)
    mkdir -p "$EXPORT_OUTPUT"
    build_libs
    ;;
  --build-all)
    mkdir -p "$EXPORT_OUTPUT"
    build_libs
    build_gui
    ;;
  --run-tests)
    run_all_tests
    ;;
  --run-benchmark)
    run_benchmark
    ;;
  *)
    help
    exit 1
    ;;
esac

info "Exports directory: $EXPORT_OUTPUT"
