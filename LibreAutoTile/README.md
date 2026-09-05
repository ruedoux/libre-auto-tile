# LibreAutoTile Core Library

Implementation of an autotile algorithm for tilemaps with JSON configuration, supporting various tile ID terrain transitions.

## Features

- Fully async-compatible
- Game engine-agnostic core library
- Isometric tile support (`TileShape.Square` / `TileShape.Isometric`)
- Tile probability for probabilistic tile selection
- Connection groups and wildcard tile IDs
- [High performance](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.Benchmarks/README.md)

## Installation

1. Link the `.csproj` from this repository (recommended for the most recent version), or
2. Install from NuGet:

```sh
dotnet add package Qwaitumin.LibreAutoTile
```

## Example usage

> Note: `MyTileMap` is your engine's tilemap implementation. The library only provides the autotiling logic.

Implement a binding for your engine:

```cs
public class MyTileMapDrawerImplementation(MyTileMap myTileMap) : ITileMapDrawer
{
  public void Clear() => myTileMap.Clear();

  public void DrawTiles(
    int tileLayer, IEnumerable<(Vector2 Position, TileData TileData)> positionsToTileData)
  {
    foreach (var (position, tileData) in positionsToTileData)
    {
      // Draw tile on the screen at 'position', on layer 'tileLayer',
      // from image file 'ImageFileName' at 'atlasPosition'
      myTileMap.DrawTile(
        tileLayer,
        tileData.TileAtlas.ImageFileName,
        position,
        tileData.TileAtlas.Position);
    }
  }
}
```

Load the configuration and compose the drawer:

```cs
AutoTileConfiguration autoTileConfiguration = AutoTileConfiguration.LoadFromFile("file/path.json");
MyTileMapDrawerImplementation drawer = new(myTileMap);

int layerCount = 1;
var tileIdToTileMaskSearcher = AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration);
AutoTiler autoTiler = new(layerCount, tileIdToTileMaskSearcher);
AutoTileDrawer autoTileDrawer = new(drawer, autoTiler);
```

Use the drawer to draw on the tilemap:

```cs
int layer = 0;
int tileId = 0;
Vector2 position = new(0, 0);

autoTileDrawer.DrawTiles(layer, [(position, tileId)]);
```

## Configuration format

Configurations are JSON files loaded with `AutoTileConfiguration.LoadFromFile`. The root object matches the `AutoTileConfiguration` type:

| Key | Type | Description |
| --- | --- | --- |
| `TileSize` | `uint` | Tile size in pixels. |
| `TileDefinitions` | `object` (map `string` → definition) | Tile definitions keyed by tile ID (as string). |
| `WildcardId` | `uint?` (optional) | A tile ID that acts as a wildcard during mask matching. Defaults to `null` (no wildcard). |
| `TileShape` | `int` (optional) | `0` = `Square` (default), `1` = `Isometric`. |

### Tile definition

| Key | Type | Description |
| --- | --- | --- |
| `ImageFileNameToTileMaskDefinition` | `object` (map file path → mask definition) | Image file path relative to the working directory, mapped to its mask definitions. |
| `Name` | `string` (optional) | Display name (default `"<NONE>"`). |
| `Color` | `string` (optional) | `"(r,g,b,a)"` used by tooling (e.g. the GUI). |
| `ConnectionGroup` | `uint?` (optional) | Group ID so distinct tile IDs can match each other's masks. |

### Mask definition

| Key | Type | Description |
| --- | --- | --- |
| `AtlasPositionToTileMaskAndChance` | `object` (map `"(x,y,z)"` → mask entries) | Atlas tile coordinate (usually `z = 0`) mapped to a list of mask entries. |

### Mask entry

| Key | Type | Description |
| --- | --- | --- |
| `Mask` | `int[8]` | The 8 neighbor tile IDs, in order: `[TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left]`. Use `-1` to match any tile (empty). |
| `Chance` | `uint` | Relative weight for weighted-random selection when multiple entries share the same mask (0 = never selected unless all are 0). |

### Example

```json
{
  "WildcardId": null,
  "TileSize": 16,
  "TileDefinitions": {
    "0": {
      "ImageFileNameToTileMaskDefinition": {
        "../resources/Grass.png": {
          "AtlasPositionToTileMaskAndChance": {
            "(0,0,0)": [
              { "Mask": [-1, -1, -1, -1, -1, -1, -1, -1], "Chance": 1 }
            ]
          }
        }
      },
      "Name": "Grass",
      "Color": "(167,58,145,178)",
      "ConnectionGroup": null
    }
  }
}
```

More complete samples live in [`resources/configurations/`](../resources/configurations), including connection groups and isometric examples.
