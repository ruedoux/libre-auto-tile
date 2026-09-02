using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class BitmaskDrawer : Node2D
{
  private readonly DrawNode bitmaskDrawNode;
  private readonly DrawNode bitmaskGhostDrawNode;

  public BitmaskDrawer()
  {
    bitmaskDrawNode = this.AppendChild(new DrawNode());
    bitmaskGhostDrawNode = this.AppendChild(new DrawNode());
  }

  public void RedrawBitmaskGhost(Vector2 worldPosition, int tileSize, Color color)
  {
    var bitmaskRectangle = GetBitmaskRectangle(worldPosition, tileSize);
    bitmaskGhostDrawNode.DrawRectangle(bitmaskRectangle, color, filled: true);
    bitmaskGhostDrawNode.QueueRedraw();
  }

  public void ShowBitmaskGhost()
    => bitmaskGhostDrawNode.Show();

  public void HideBitmaskGhost()
    => bitmaskGhostDrawNode.Hide();

  public void RedrawBitmask(Dictionary<Rect2I, Color> bitmaskRectangleToColor)
  {
    foreach (var (rectangle, color) in bitmaskRectangleToColor)
      bitmaskDrawNode.DrawRectangle(rectangle, color, filled: true);
    bitmaskDrawNode.QueueRedraw();
  }

  public static Rect2I GetBitmaskRectangle(Vector2 worldPosition, int tileSize)
  {
    var snappedTilePosition = TileSetMath.SnapToTileCorner(worldPosition, tileSize);
    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, tileSize);
    return TileSetMath.SnappedBitmaskPositionToWorldRectangle(
      snappedTilePosition, bitmaskPosition, tileSize);
  }
}
