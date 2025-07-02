using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;

namespace Qwaitumin.LibreAutoTile.GodotExample;

public partial class ProceduralExample : Node2D
{
  enum TILES { GRASS = 0, WATER = 1 }

  const string CONFIG_PATH = "../resources/configurations/ExampleConfigurationTransient.json";
  const int LAYER = 0;

  public override void _Ready()
  {
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(2, autoTileConfiguration);
    AddChild(autoTileMap);

    FastNoiseLite noise = new()
    {
      NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex,
      FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
      FractalOctaves = 1,
      Frequency = 0.1f,
      Seed = new Random().Next()
    };

    Dictionary<Vector2I, int> positionToTileId = [];
    for (int x = 0; x < 64; x++)
      for (int y = 0; y < 64; y++)
        if (x > 0 && y > 0 && x < 63 && y < 63)
          positionToTileId[new(x, y)] = noise.GetNoise2D(x, y) < 0.5 ? (int)TILES.GRASS : (int)TILES.WATER;
        else
          positionToTileId[new(x, y)] = (int)TILES.WATER;

    autoTileMap.DrawTiles(
      LAYER, positionToTileId.Select(kv => (Position: kv.Key, TileId: kv.Value)).ToArray());
  }
}