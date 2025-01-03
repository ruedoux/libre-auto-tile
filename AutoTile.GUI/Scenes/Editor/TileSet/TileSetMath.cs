using System;
using Godot;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;

public static class TileSetMath
{
  public static Vector2I SnapToTileCorner(Vector2 worldPosition, int tileSize)
    => ScaleDownTilePosition(worldPosition, tileSize) * tileSize;

  public static Vector2I ScaleDownTilePosition(Vector2 worldPosition, int tileSize)
  {
    int tileXScaledDown = (int)Math.Floor(worldPosition.X / tileSize);
    int tileYScaledDown = (int)Math.Floor(worldPosition.Y / tileSize);
    return new Vector2I(tileXScaledDown, tileYScaledDown);
  }
}