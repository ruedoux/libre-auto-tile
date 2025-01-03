# LibreAutoTile Core Library

Implementation of an autotile algorithm for tilemaps with JSON configuration, supporting various tile ID terrain transitions.

## Features

- Autotiling pipeline
- Fully async-compatible
- Game engine-agnostic core library
- [High performance](https://github.com/ruedoux/libre-auto-tile/blob/main/LibreAutoTile.Benchmarks/README.md)

## Example usage

> Note: `MyTileMap` is your engine’s tilemap implementation. The library only provides the autotiling logic.

Implement a binding for your engine:

```cs
public class MyTileMapDrawerImplementation(MyTileMap myTileMap) : ITileMapDrawer
{
  public void Clear() => myTileMap.Clear();

  public void DrawTiles(
    int tileLayer, IEnumerable<(Configuration.Vector2, TileData)> positionsToTileData)
  {
    foreach (var (position, tileData) in positionsToTileData)
    {
      // Draw tile on the screen at 'position', on layer 'tileLayer', from image file 'ImageFileName' at 'atlasPosition'
      myTileMap.DrawTile(
        tileLayer,
        tileData.TileAtlas.ImageFileName,
        position,
        tileData.TileAtlas.Position);
    }
  }
}
```

Load configuration and compose drawer:

```cs
AutoTileConfiguration autoTileConfiguration = AutoTileConfiguration.LoadFromFile("file/path.json");
MyTileMapDrawerImplementation drawer = new(myTileMap);

AutoTilerComposer autoTilerComposer = new(autoTileConfiguration);
var autoTileDrawer = new AutoTileDrawer(drawer, autoTilerComposer.GetAutoTiler(layerCount));
```

Use drawer to draw on TileMap:

```cs
int layer = 0;
int tileId = 0;
Vector2 position = new(0,0);

autoTileDrawer.DrawTiles(layer, [(position, tileId)])
```
