using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Qwaitumin.AutoTile.GUI.Core.GodotBindings;
using Qwaitumin.AutoTile.GUI.Scenes.Utils;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;

public partial class TileSetBitmaskDrawer : Node2D
{
  private readonly DrawNode bitmaskDrawNode;
  private readonly DrawNode bitmaskGhostDrawNode;

  public TileSetBitmaskDrawer()
  {
    bitmaskDrawNode = GodotApi.AddChild<DrawNode>(this, new());
    bitmaskGhostDrawNode = GodotApi.AddChild<DrawNode>(this, new());
  }

  public void ClearAllDrawn()
  {
    bitmaskDrawNode.QueueRedraw();
    bitmaskGhostDrawNode.QueueRedraw();
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

  public static Rect2I GetBitmaskRectangle(Vector2I worldPosition, int tileSize)
    => GetBitmaskRectangle(new Vector2(worldPosition.X, worldPosition.Y), tileSize);

  public static Rect2I GetBitmaskRectangle(Vector2 worldPosition, int tileSize)
  {
    var snappedTilePosition = TileSetMath.SnapToTileCorner(worldPosition, tileSize);

    int distanceX = Math.Abs((int)worldPosition.X - snappedTilePosition.X);
    int distanceY = Math.Abs((int)worldPosition.Y - snappedTilePosition.Y);
    Vector2I pointInTile = new(distanceX, distanceY);

    Rect2I[,] ranges = GetBitmaskRectanglesInsideTile(tileSize);
    Rect2I rectangle = ranges.Cast<Rect2I>().FirstOrDefault(
      range => range.HasPoint(pointInTile));

    return new(rectangle.Position + snappedTilePosition, rectangle.Size);
  }

  private static Rect2I[,] GetBitmaskRectanglesInsideTile(int tileSize)
  {
    int reminder = tileSize % 3;
    int bitmaskWidth = tileSize / 3;

    Rect2I[,] ranges = new Rect2I[3, 3];
    for (int y = 0; y < 3; y++)
    {
      for (int x = 0; x < 3; x++)
      {
        int width = bitmaskWidth + (x == 2 ? reminder : 0);
        int height = bitmaskWidth + (y == 2 ? reminder : 0);
        ranges[x, y] = new Rect2I(new(x * bitmaskWidth, y * bitmaskWidth), new(width, height));
      }
    }

    return ranges;
  }
}