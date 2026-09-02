using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class GridDrawer : Node2D
{
  private readonly DrawNode gridDrawNode;
  private readonly DrawNode tileDrawNode;

  public GridDrawer()
  {
    gridDrawNode = this.AppendChild(new DrawNode());
    tileDrawNode = this.AppendChild(new DrawNode());
  }

  public void RedrawSquareGrid(Rect2I size, Color color, int tileSize)
  {
    int startX = size.Position.X;
    int startY = size.Position.Y;
    int endX = size.End.X;
    int endY = size.End.Y;
    var borderWidth = TileSetMath.BorderWidth(tileSize);

    for (int x = startX; x <= endX; x += tileSize)
      gridDrawNode.DrawSimpleLine(new Vector2I(x, startY), new Vector2I(x, endY), color, width: borderWidth);

    for (int y = startY; y <= endY; y += tileSize)
      gridDrawNode.DrawSimpleLine(new Vector2I(startX, y), new Vector2I(endX, y), color, width: borderWidth);

    gridDrawNode.QueueRedraw();
  }

  public void RedrawSquareTile(Vector2I snappedTilePosition, Color color, int tileSize, bool filled = false)
  {
    Rect2I tileRect = new(snappedTilePosition, new(tileSize, tileSize));

    var borderWidth = TileSetMath.BorderWidth(tileSize);
    tileDrawNode.DrawRectangle(tileRect, color, width: borderWidth, filled: filled);
    tileDrawNode.QueueRedraw();
  }
}
