using System;
using System.Collections.Generic;
using Godot;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes.Comparasion;

public static class Comparasion
{
  public enum TILES { GRASS = 0, WATER = 1 }

  public static Dictionary<Vector2I, int> GetPositionToTileId(Vector2I dimensions)
  {
    Random random = new();
    Dictionary<Vector2I, int> positionToTileId = [];
    for (int x = 0; x < dimensions.X; x++)
      for (int y = 0; y < dimensions.X; y++)
        if (x > 0 && y > 0 && x < dimensions.X - 1 && y < dimensions.X - 1)
          positionToTileId[new(x, y)] = random.Next(0, 2) == 0 ? (int)TILES.GRASS : (int)TILES.WATER;
        else
          positionToTileId[new(x, y)] = (int)TILES.WATER;
    return positionToTileId;
  }
}