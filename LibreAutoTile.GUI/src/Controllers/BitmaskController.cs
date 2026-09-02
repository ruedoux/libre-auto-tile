using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class BitmaskController
{
  private readonly EditorContext context;

  public event Action<int>? LayerChanged;

  public BitmaskController(EditorContext context)
  {
    this.context = context;
    context.View.LayerControl.LayerSpinBox.ValueChanged += OnLayerSpinBoxChanged;
  }

  private void OnLayerSpinBoxChanged(double value)
    => LayerChanged?.Invoke(Math.Max(0, (int)value));

  public void HandleMouseInput(InputEventMouse inputEventMouse)
  {
    if (context.ActiveTool != EditorTool.Tiles)
      return;
    if (GodotExtensions.IsMouseOnElements(context.UiElements))
      return;

    var mousePosition = context.View.GetGlobalMousePosition();
    var mousePositionInt = new Vector2I((int)mousePosition.X, (int)mousePosition.Y);

    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;

    if (!new Rect2I(Vector2I.Zero, context.Data.ImageSize).HasPoint(mousePositionInt))
      return;

    if (mouseRightClicked)
      RemoveBitmaskSegment(mousePositionInt);
    if (mouseLeftClicked && context.Data.Tiles.ActiveTile is not null)
      AddBitmaskSegment(context.Data.Tiles.ActiveTile.TileId, mousePositionInt);
    if (mouseRightClicked || mouseLeftClicked)
      context.RedrawBitmask();
  }

  private void AddBitmaskSegment(int tileId, Vector2 worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.ScaledTileSize);
    context.Data.BitmaskDatabase.CreateBitmaskData(context.Data.ImagePath, scaledTilePosition);
    var bitmaskData = context.Data.BitmaskDatabase.GetBitmaskData(context.Data.ImagePath, scaledTilePosition)
      ?? throw new NullReferenceException("BitmaskData is null");
    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, context.ScaledTileSize);
    bitmaskData.AddBitmask(context.Data.CurrentLayer, tileId, bitmaskPosition);
    context.RefreshImageOptions();
  }

  private void RemoveBitmaskSegment(Vector2 worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.ScaledTileSize);
    var bitmaskData = context.Data.BitmaskDatabase.GetBitmaskData(context.Data.ImagePath, scaledTilePosition);
    if (bitmaskData is null) return;

    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, context.ScaledTileSize);
    bitmaskData.RemoveBitmask(context.Data.CurrentLayer, bitmaskPosition);

    if (bitmaskData.IsEmpty())
    {
      context.Data.BitmaskDatabase.RemoveBitmaskData(context.Data.ImagePath, scaledTilePosition);
      context.View.TileProbability.ChangeLabelProbability(scaledTilePosition, 1);
    }
    context.RefreshImageOptions();
  }
}
