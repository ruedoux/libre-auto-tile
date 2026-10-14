# LibreAutoTile Godot Bindings

Godot Bindings implementation for LibreAutoTile. Example usage in a live project [here](../LibreAutoTile.GodotExample).

## Installation

Link the `.csproj` from this repository (recommended for the most recent version).

## Example Usage

```cs
var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
AutoTileMap autoTileMap = new(1, autoTileConfiguration);
AddChild(autoTileMap);

int tileId = 0;
int layer = 0;
autoTileMap.DrawTiles(layer, [(new Vector2I(0,0), tileId)]);
```

> Note: `AutoTileMap` must be constructed on the main thread — it creates Godot
> Resources and Nodes (image loading, TileSet/atlas sources, `CreateTile`, `AddChild`).

## Isometric

Pass a tile shape to render on an isometric (diamond) tile map. The core autotiling
logic is shape-agnostic, so only the Godot `TileSet` configuration changes:

```cs
AutoTileMap autoTileMap = new(
  1, autoTileConfiguration, TileShape.Isometric);
AddChild(autoTileMap);
```

> `tileShape` defaults to the shape stored in the configuration
> (`autoTileConfiguration.TileShape`).
