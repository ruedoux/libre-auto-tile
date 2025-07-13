# LibreAutoTile

Implementation of an autotile algorithm for tilemaps with JSON configuration, supporting various tile ID terrain transitions.

![GUI Demo](https://github.com/ruedoux/libre-auto-tile/blob/main/resources/gui.gif?raw=true)

## Features

- Autotiling pipeline
- Fully async-compatible
- Dedicated GUI for configuration
- Game engine-agnostic core library
- [High performance](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.Benchmarks/README.md)

## Usage

For documentation, see `README.md` files in subdirectories:

- [Core library](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile)
- [Godot bindings](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.GodotBindings/README.md)

A dedicated [GUI](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile.GUI) is available. Compiled binaries are in [Releases](https://github.com/ruedoux/libre-auto-tile/releases).

## Installation

1. Link the `.csproj` from this repository (for latest version), or
2. Install from NuGet:

```sh
# Godot bindings
dotnet add package Qwaitumin.LibreAutoTile.GodotBindings
# Core library
dotnet add package Qwaitumin.LibreAutoTile
```

> Library targets:

```xml
<TargetFramework>net9.0</TargetFramework>
<LangVersion>12.0</LangVersion>
```

## Compilation

Use the `build-release.sh` script. On Windows use WSL or compile each project manually.

## Game Engine Integration

Currently, only Godot engine bindings are supported. Contributions for other game engine bindings are welcome.

Example usage in a live project [here](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile.GodotExample/Scenes/Examples).

Library bindings draw terrains [10x](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile.GodotExample/Scenes/Comparasion) faster than godot terrain implementation.

## Contributions

Anyone is free to contribute. Before creating a large component, please open an issue to ensure it aligns with the project's direction.

## Additional projects

- Check out [better-terrain](https://github.com/Portponky/better-terrain) for integrated Godot addon
