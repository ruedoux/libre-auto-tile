namespace LibreAutoTile.Configuration.Models;

/// <summary>
/// Tile shape used for rendering. Determines how tile dimensions map to a Godot
/// TileSet and, eventually, the neighbor topology (hex) used by the autotiler.
/// </summary>
public enum TileShape
{
  Square,
  Isometric,
}

public static class TileShapeExtensions
{
  public static Vector2 GetCellSize(this TileShape shape, int tileSize)
    => shape == TileShape.Isometric
      ? new Vector2(tileSize, Math.Max(1, tileSize / 2))
      : new Vector2(tileSize, tileSize);
}
