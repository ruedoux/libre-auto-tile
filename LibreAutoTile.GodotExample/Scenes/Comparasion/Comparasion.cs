using System;
using System.Collections.Generic;
using Godot;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes;

public static class Comparasion
{
  public readonly static Vector2I MAP_SIZE = new(512, 512);
  public enum TILES { GRASS = 0, WATER = 1 }

  public static Dictionary<Vector2I, int> GetPositionToTileId()
  {
    Random random = new();
    Dictionary<Vector2I, int> positionToTileId = [];
    for (int x = 0; x < MAP_SIZE.X; x++)
      for (int y = 0; y < MAP_SIZE.X; y++)
        if (x > 0 && y > 0 && x < MAP_SIZE.X - 1 && y < MAP_SIZE.X - 1)
          positionToTileId[new(x, y)] = random.Next(0, 2) == 0 ? (int)TILES.GRASS : (int)TILES.WATER;
        else
          positionToTileId[new(x, y)] = (int)TILES.WATER;
    return positionToTileId;
  }
}