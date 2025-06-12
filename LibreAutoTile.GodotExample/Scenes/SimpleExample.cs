using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;

namespace Qwaitumin.LibreAutoTile.GodotExample;

public partial class SimpleExample : Node2D
{
  enum TILES { GRASS = 0, WATER = 1 }

  const string CONFIG_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  const int LAYER = 0;

  public override void _Ready()
  {
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(2, autoTileConfiguration);
    AddChild(autoTileMap);
    autoTileMap.Scale = new(2, 2);

    Random random = new();
    Dictionary<Vector2I, int> positionToTileId = [];
    for (int x = 0; x < 32; x++)
      for (int y = 0; y < 32; y++)
        if (x > 0 && y > 0 && x < 31 && y < 31)
          positionToTileId[new(x, y)] = random.Next(0, 2) == 0 ? (int)TILES.GRASS : (int)TILES.WATER;
        else
          positionToTileId[new(x, y)] = (int)TILES.WATER;

    autoTileMap.DrawTiles(
      LAYER, positionToTileId.Select(kv => (Position: kv.Key, TileId: kv.Value)).ToArray());
  }
}