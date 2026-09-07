using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class ProbabilityController
{
  private readonly EditorContext context;

  private Vector2I? selectedProbabilityTile;

  public bool HasSelection => selectedProbabilityTile is not null;

  public ProbabilityController(EditorContext context)
  {
    this.context = context;
    context.EditorScene.ProbabilityPanel.ProbabilitySpinBox.ValueChanged += OnProbabilitySpinBoxChanged;
  }

  public void EnterProbability()
  {
    context.EditorScene.TileProbability.Show();
    context.RedrawProbabilityLabels();
    selectedProbabilityTile = context.EditorData.ImagePath == "" ? null : Vector2I.Zero;
    SyncSpinBox();
    RedrawSelection();
  }

  public void ExitProbability()
  {
    context.EditorScene.TileProbability.Hide();
    selectedProbabilityTile = null;
    context.EditorScene.ClearProbabilitySelection();
    SyncSpinBox();
  }

  public void HandleMouseInput(InputEventMouse inputEventMouse)
  {
    if (context.ActiveTool != EditorTool.Probability)
      return;
    if (GodotExtensions.IsMouseOnElements(context.UiElements))
      return;

    var mousePosition = context.EditorScene.GetGlobalMousePosition();
    var mousePositionInt = new Vector2I((int)mousePosition.X, (int)mousePosition.Y);

    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;
    if (mouseLeftClicked)
      SelectProbabilityTile(mousePositionInt);
  }

  private void SelectProbabilityTile(Vector2I worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.ScaledTileSize);
    var bitmaskData = context.EditorData.BitmaskDatabase.GetBitmaskData(context.EditorData.ImagePath, scaledTilePosition);
    if (bitmaskData is null || bitmaskData.IsEmpty())
      return;

    selectedProbabilityTile = scaledTilePosition;
    SyncSpinBox();
    RedrawSelection();
  }

  private void OnProbabilitySpinBoxChanged(double value)
    => SetSelectedProbability((uint)Math.Clamp((long)value, 0L, uint.MaxValue));

  public void AdjustSelectedProbability(int delta)
  {
    if (!TryGetSelectedBitmaskData(out var bitmaskData))
      return;

    long currentValue = bitmaskData.GetProbability(context.EditorData.CurrentLayer);
    SetSelectedProbability((uint)Math.Clamp(currentValue + delta, 0L, uint.MaxValue));
  }

  private void SetSelectedProbability(uint value)
  {
    if (!TryGetSelectedBitmaskData(out var bitmaskData))
      return;

    bitmaskData.SetProbability(context.EditorData.CurrentLayer, value);
    context.EditorScene.TileProbability.ChangeLabelProbability(selectedProbabilityTile!.Value, value);
    context.EditorScene.ProbabilityPanel.SetProbabilityValue(value);
  }

  private bool TryGetSelectedBitmaskData(out BitmaskData bitmaskData)
  {
    bitmaskData = null!;
    if (selectedProbabilityTile is null)
      return false;

    var result = context.EditorData.BitmaskDatabase.GetBitmaskData(context.EditorData.ImagePath, selectedProbabilityTile.Value);
    if (result is null || result.IsEmpty())
      return false;

    bitmaskData = result;
    return true;
  }

  public void SyncSpinBox()
  {
    context.EditorScene.ProbabilityPanel.SetSelectedPosition(selectedProbabilityTile);
    bool hasData = TryGetSelectedBitmaskData(out var bitmaskData);
    context.EditorScene.ProbabilityPanel.SetProbabilityEnabled(hasData);
    if (hasData)
      context.EditorScene.ProbabilityPanel.SetProbabilityValue(bitmaskData.GetProbability(context.EditorData.CurrentLayer));
  }

  public void RedrawSelection()
  {
    if (selectedProbabilityTile is null)
    {
      context.EditorScene.ClearProbabilitySelection();
      return;
    }

    context.EditorScene.RedrawProbabilitySelection(
      selectedProbabilityTile.Value * context.ScaledTileSize, context.AppearanceSettings.SelectionColor, context.ScaledTileSize);
  }

  public void ResetSelection()
  {
    selectedProbabilityTile = null;
    context.EditorScene.ClearProbabilitySelection();
    SyncSpinBox();
  }
}
