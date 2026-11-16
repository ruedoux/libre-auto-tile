using Godot;
using LibreAutoTile.GUI.Editor.Model;
using TileShape = LibreAutoTile.Configuration.Models.TileShape;

namespace LibreAutoTile.GUI.Editor.View.Drawing;

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
    gridDrawNode.Clear();

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

  public void RedrawTile(
    Vector2I snappedTilePosition, Color color, int tileSize, TileShape shape, bool filled = false)
  {
    tileDrawNode.Clear();

    var borderWidth = TileSetMath.BorderWidth(tileSize);
    if (shape == TileShape.Isometric)
    {
      // Center the 2:1 isometric diamond in the square atlas cell.
      Vector2 tileTopLeft = snappedTilePosition + new Vector2(0, tileSize / 4f);
      var vertices = TileSetMath.GetTileOutlineVertices(tileTopLeft, tileSize, shape);
      tileDrawNode.DrawPolygon(vertices, color, filled: filled, width: borderWidth);
    }
    else
    {
      Rect2I tileRect = new(snappedTilePosition, new Vector2I(tileSize, tileSize));
      tileDrawNode.DrawRectangle(tileRect, color, width: borderWidth, filled: filled);
    }
    tileDrawNode.QueueRedraw();
  }
}
