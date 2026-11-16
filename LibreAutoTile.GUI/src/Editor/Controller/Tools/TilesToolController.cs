using Godot;
using LibreAutoTile.GUI.Editor.Model;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public class TilesToolController : IToolState
{
  private readonly EditorContext context;

  public event Action<int>? LayerChanged;

  public TilesToolController(EditorContext context)
  {
    this.context = context;

    context.View.LayerControl.LayerSpinBox.ValueChanged += OnLayerSpinBoxChanged;
  }

  private void OnLayerSpinBoxChanged(double value)
    => LayerChanged?.Invoke(Math.Max(0, (int)value));

  public void HandleMouseInput(InputEventMouse inputEventMouse, Vector2 position)
  {
    var mousePositionInt = new Vector2I((int)position.X, (int)position.Y);

    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;

    if (!new Rect2I(Vector2I.Zero, context.Model.ImageSize).HasPoint(mousePositionInt))
      return;

    if (mouseRightClicked)
      RemoveBitmaskSegment(mousePositionInt);
    if (mouseLeftClicked && context.Model.Tiles.ActiveTile is not null)
      AddBitmaskSegment(context.Model.Tiles.ActiveTile.TileId, mousePositionInt);
    if (mouseRightClicked || mouseLeftClicked)
      context.Redrawer.RedrawBitmask();
  }

  public void HandleMouseMotion(Vector2 position)
  {
    context.Redrawer.DrawTileHover(position);
    RedrawBitmaskGhost(position);
  }

  private void RedrawBitmaskGhost(Vector2 worldPosition)
    => context.View.Workspace.BitmaskDrawer.RedrawBitmaskGhost(
      worldPosition, context.Redrawer.ScaledTileSize, new(r: 255f, g: 255f, b: 255f, a: 0.5f));

  private void AddBitmaskSegment(int tileId, Vector2 worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.Redrawer.ScaledTileSize);
    context.Model.BitmaskDatabase.CreateBitmaskData(context.Model.ImagePath, scaledTilePosition);
    var bitmaskData = context.Model.BitmaskDatabase.GetBitmaskData(context.Model.ImagePath, scaledTilePosition)
      ?? throw new NullReferenceException("BitmaskData is null");
    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, context.Redrawer.ScaledTileSize);
    bitmaskData.AddBitmask(context.Model.CurrentLayer, tileId, bitmaskPosition);
    context.Redrawer.RefreshImageOptions();
  }

  private void RemoveBitmaskSegment(Vector2 worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.Redrawer.ScaledTileSize);
    var bitmaskData = context.Model.BitmaskDatabase.GetBitmaskData(context.Model.ImagePath, scaledTilePosition);
    if (bitmaskData is null)
      return;

    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, context.Redrawer.ScaledTileSize);
    bitmaskData.RemoveBitmask(context.Model.CurrentLayer, bitmaskPosition);

    if (bitmaskData.IsEmpty())
    {
      context.Model.BitmaskDatabase.RemoveBitmaskData(context.Model.ImagePath, scaledTilePosition);
      context.View.Workspace.TileProbability.ChangeLabelProbability(scaledTilePosition, 1);
    }
    context.Redrawer.RefreshImageOptions();
  }
}
