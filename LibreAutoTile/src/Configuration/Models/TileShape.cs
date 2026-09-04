namespace Qwaitumin.LibreAutoTile.Configuration.Models;

/// <summary>
/// Tile shape used for rendering. Determines how tile dimensions map to a Godot
/// TileSet and, eventually, the neighbor topology (hex) used by the autotiler.
/// </summary>
public enum TileShape
{
  Square,
  Isometric,
}
