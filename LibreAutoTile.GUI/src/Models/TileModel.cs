using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public class TileModel
{
  public int TileId { get; set; }
  public string TileName { get; set; } = "";
  public Color Color { get; set; }
  public uint? ConnectionGroup { get; set; }
}
