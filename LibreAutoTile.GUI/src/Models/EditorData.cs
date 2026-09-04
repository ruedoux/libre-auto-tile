using Godot;
using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public class EditorData
{
  public string ImagePath { get; set; } = "";
  public Vector2I ImageSize { get; set; } = Vector2I.Zero;
  public int TileSize { get; set; } = 16;
  public TileShape TileShape { get; set; } = TileShape.Square;
  public int CurrentLayer { get; set; } = 0;
  public readonly BitmaskDatabase BitmaskDatabase = new();
  public readonly TileCollection Tiles = new();
}
