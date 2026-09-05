#!/bin/bash

godot --path=. --headless --import
dotnet build Qwaitumin.LibreAutoTile.GodotBindings.Tests.csproj

for arg in "$@"; do
  case $arg in
    --test-method=*)
      TEST_METHOD="--test-method=${arg#*=}"
      ;;
    --test-class=*)
      TEST_CLASS="--test-class=${arg#*=}"
      ;;
  esac
done

godot --path=. --headless $TEST_CLASS $TEST_METHOD
