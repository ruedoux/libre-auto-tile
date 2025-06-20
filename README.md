# LibreAutoTile

Implementation of an autotile algorithm for tilemaps with JSON configuration, supporting various tile ID terrain transitions.

![GUI Demo](https://github.com/ruedoux/libre-auto-tile/blob/main/resources/gui.gif?raw=true)

## Features

- Autotiling pipeline
- Fully async-compatible
- Dedicated GUI for AutoTile configuration
- Game engine-agnostic core library
- [High performance](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.Benchmarks/README.md)

## Usage

For documentation about the core library or specific bindings, see the `README.md` files located in subdirectories:

- [Core library](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile)
- [Godot bindings](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.GodotBindings/README.md)

For creating configuration files, there is a dedicated [GUI](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile.GUI). A compiled GUI is available in [Releases](https://github.com/ruedoux/libre-auto-tile/releases).

## Installation

1. Link the `.csproj` from this repository (recommended for the most recent version)
2. Get bindings or the core library via NuGet:

```sh
# For godot bindings (https://www.nuget.org/packages/Qwaitumin.LibreAutoTile.GodotBindings/)
dotnet add package Qwaitumin.LibreAutoTile.GodotBindings --version 1.0.0-alpha.1
```

```sh
# For core library (https://www.nuget.org/packages/Qwaitumin.LibreAutoTile/)
dotnet add package Qwaitumin.LibreAutoTile --version 1.0.0-alpha.3
```

> Note: Library is meant for:

```xml
<TargetFramework>net9.0</TargetFramework>
<LangVersion>12.0</LangVersion>
```

## Game Engine Integration

Currently, only Godot engine bindings are supported. Contributions for other game engine bindings are welcome.

## Contributions

Anyone is free to contribute. Before creating a large component, please open an issue to ensure it aligns with the project's direction.
