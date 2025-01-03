using System;
using Godot;
using Qwaitumin.AutoTile.GUI.Core.GodotBindings;
using Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;
using Qwaitumin.AutoTile.GUI.Scenes.Utils;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Utils;

public partial class EditorTileDrawer : Node2D
{
  private readonly DrawNode tileDrawNode;
  private readonly DrawNode gridDrawNode;
  private readonly DrawNode bitmaskOutlineDrawNode;

  public EditorTileDrawer()
  {
    gridDrawNode = GodotApi.AddChild<DrawNode>(this, new());
    tileDrawNode = GodotApi.AddChild<DrawNode>(this, new());
    bitmaskOutlineDrawNode = GodotApi.AddChild<DrawNode>(this, new());
  }

  public void ShowSelectedTile()
  {
    tileDrawNode.Show();
    bitmaskOutlineDrawNode.Show();
  }

  public void HideSelectedTile()
  {
    tileDrawNode.Hide();
    bitmaskOutlineDrawNode.Hide();
  }

  public void RedrawSelectedBitmaskOutline(Rect2I bitmaskRectangle, Color color)
  {
    bitmaskOutlineDrawNode.DrawRectangle(bitmaskRectangle, color, filled: true);
    bitmaskOutlineDrawNode.QueueRedraw();
  }

  public void RedrawTile(Vector2 worldPosition, Color color, int tileSize, bool filled = false)
  {
    var snappedTilePosition = TileSetMath.SnapToTileCorner(worldPosition, tileSize);
    Rect2I tileRect = new(snappedTilePosition, new(tileSize, tileSize));

    var borderWidth = GetBorderWidth(tileSize);
    tileDrawNode.DrawRectangle(tileRect, color, width: borderWidth, filled: filled);
    tileDrawNode.QueueRedraw();
  }

  public void RedrawGrid(Rect2I size, Color color, int tileSize)
  {
    int startX = size.Position.X;
    int startY = size.Position.Y;
    int endX = size.End.X;
    int endY = size.End.Y;
    var borderWidth = GetBorderWidth(tileSize);

    for (int x = startX; x <= endX; x += tileSize)
      gridDrawNode.DrawSimpleLine(new Vector2I(x, startY), new Vector2I(x, endY), color, width: borderWidth);

    for (int y = startY; y <= endY; y += tileSize)
      gridDrawNode.DrawSimpleLine(new Vector2I(startX, y), new Vector2I(endX, y), color, width: borderWidth);

    gridDrawNode.QueueRedraw();
  }

  private static int GetBorderWidth(int tileSize)
    => (tileSize / 32) > 0 ? (tileSize / 32) : 1;
}