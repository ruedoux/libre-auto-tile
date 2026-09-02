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
    context.View.ProbabilityPanel.ProbabilitySpinBox.ValueChanged += OnProbabilitySpinBoxChanged;
  }

  public void EnterProbability()
  {
    context.View.TileProbability.Show();
    context.RedrawProbabilityLabels();
    selectedProbabilityTile = context.Data.ImagePath == "" ? null : Vector2I.Zero;
    SyncSpinBox();
    RedrawSelection();
  }

  public void ExitProbability()
  {
    context.View.TileProbability.Hide();
    selectedProbabilityTile = null;
    context.View.ClearProbabilitySelection();
    SyncSpinBox();
  }

  public void HandleMouseInput(InputEventMouse inputEventMouse)
  {
    if (context.ActiveTool != EditorTool.Probability)
      return;
    if (GodotExtensions.IsMouseOnElements(context.UiElements))
      return;

    var mousePosition = context.View.GetGlobalMousePosition();
    var mousePositionInt = new Vector2I((int)mousePosition.X, (int)mousePosition.Y);

    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;
    if (mouseLeftClicked)
      SelectProbabilityTile(mousePositionInt);
  }

  private void SelectProbabilityTile(Vector2I worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.ScaledTileSize);
    var bitmaskData = context.Data.BitmaskDatabase.GetBitmaskData(context.Data.ImagePath, scaledTilePosition);
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

    long currentValue = bitmaskData.GetProbability(context.Data.CurrentLayer);
    SetSelectedProbability((uint)Math.Clamp(currentValue + delta, 0L, uint.MaxValue));
  }

  private void SetSelectedProbability(uint value)
  {
    if (!TryGetSelectedBitmaskData(out var bitmaskData))
      return;

    bitmaskData.SetProbability(context.Data.CurrentLayer, value);
    context.View.TileProbability.ChangeLabelProbability(selectedProbabilityTile!.Value, value);
    context.View.ProbabilityPanel.SetProbabilityValue(value);
  }

  private bool TryGetSelectedBitmaskData(out BitmaskData bitmaskData)
  {
    bitmaskData = null!;
    if (selectedProbabilityTile is null)
      return false;

    var result = context.Data.BitmaskDatabase.GetBitmaskData(context.Data.ImagePath, selectedProbabilityTile.Value);
    if (result is null || result.IsEmpty())
      return false;

    bitmaskData = result;
    return true;
  }

  public void SyncSpinBox()
  {
    context.View.ProbabilityPanel.SetSelectedPosition(selectedProbabilityTile);
    bool hasData = TryGetSelectedBitmaskData(out var bitmaskData);
    context.View.ProbabilityPanel.SetProbabilityEnabled(hasData);
    if (hasData)
      context.View.ProbabilityPanel.SetProbabilityValue(bitmaskData.GetProbability(context.Data.CurrentLayer));
  }

  public void RedrawSelection()
  {
    if (selectedProbabilityTile is null)
    {
      context.View.ClearProbabilitySelection();
      return;
    }

    context.View.RedrawProbabilitySelection(
      selectedProbabilityTile.Value * context.ScaledTileSize, context.Appearance.SelectionColor, context.ScaledTileSize);
  }

  public void ResetSelection()
  {
    selectedProbabilityTile = null;
    context.View.ClearProbabilitySelection();
    SyncSpinBox();
  }
}
