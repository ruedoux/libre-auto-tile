# LibreAutoTile Godot Bindings

Godot Bindings implementation for LibreAutoTile. Example usage on live project [here](https://github.com/ruedoux/libre-auto-tile/tree/main/LibreAutoTile.GodotExample).

## Installation

1. Using .csproj repository from repo (recommended for most recent version)
2. Using NuGet:

```sh
dotnet add package Qwaitumin.LibreAutoTile.GodotBindings
```

## Example Usage

```cs
var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
AutoTileMap autoTileMap = new(1, autoTileConfiguration);
AddChild(autoTileMap);

int tileId = 0;
int layer = 0;
autoTileMap.DrawTiles(layer, [(new Vector2I(0,0), tileId)]);
```
