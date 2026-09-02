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
  godot --version --quit

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

  if [[ -z "${PACKAGE_VERSION:-}" ]]; then
      PACKAGE_VERSION="0.0.0-debug"
  fi

  dotnet restore
  dotnet pack LibreAutoTile -c Release -p:PackageVersion="$PACKAGE_VERSION"
  dotnet pack LibreAutoTile.GodotBindings -c Release -p:PackageVersion="$PACKAGE_VERSION"
  cp LibreAutoTile/bin/Release/*.nupkg "$EXPORT_OUTPUT"/
  cp LibreAutoTile.GodotBindings/bin/Release/*.nupkg "$EXPORT_OUTPUT"/

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

publish() {
    info "Attempting to publish version $PACKAGE_VERSION"
    info "Meant only for maintainers use"

    if [[ -z "$NUGET_API_KEY" ]]; then
        error "NUGET_API_KEY is not set."
        exit 1
    fi

    if [[ ! -f release-notes.md ]]; then
        error "release-notes.md not found."
        exit 1
    fi

    if ! gh auth status &> /dev/null; then
        error "GitHub CLI (gh) not authenticated."
        exit 1
    fi

    for pkg in "$EXPORT_OUTPUT"/*.nupkg; do
        dotnet nuget push "$pkg" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
    done

    gh release create "$PACKAGE_VERSION" "$EXPORT_OUTPUT"/* --title "$PACKAGE_VERSION" --notes-file release-notes.md
}

publish_release() {
    if [[ -z "${2:-}" ]]; then
        echo "Usage: $0 --publish <version>"
        exit 1
    fi

    PACKAGE_VERSION="$2"
    if [[ ! "$PACKAGE_VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z0-9._]+)?$ ]]; then
        error "Package version must match x.x.x or x.x.x-suffix"
        exit 1
    fi

    rm -rf ./build
    mkdir -p "$EXPORT_OUTPUT"
    build_libs
    build_gui
    run_benchmark

    if [[ "${CI:-}" == "true" ]]; then
        # CI detected, skip prompts
        confirm="y"
    else
        read -p "Are you sure you want to publish version '$2'? [y/N] " confirm
    fi
    if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
        echo "Aborted."
        exit 1
    fi

    publish "$2"
}

help() {
  echo "Usage: $0 [--build-gui|--build-libs|--build-all|--run-tests|--run-benchmark|--publish <version>]"
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
  --publish)
    publish_release "$@"
    ;;
  *)
    help
    exit 1
    ;;
esac

info "Exports directory: $EXPORT_OUTPUT"
