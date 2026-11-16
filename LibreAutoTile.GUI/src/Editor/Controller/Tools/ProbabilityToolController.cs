using Godot;
using LibreAutoTile.GUI.Editor.Model;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public class ProbabilityToolController : IToolState
{
  private readonly EditorContext context;

  private Vector2I? selectedProbabilityTile;

  public bool HasSelection => selectedProbabilityTile is not null;

  public ProbabilityToolController(EditorContext context)
  {
    this.context = context;

    context.View.ToolsPanel.ProbabilityPanel.ProbabilitySpinBox.ValueChanged += OnProbabilitySpinBoxChanged;

    context.DataChanged += ResetSelection;
    context.TileGeometryChanged += RedrawSelection;
  }

  public bool CanActivate() => context.Model.ImagePath != "";
  public string ActivationError => "No tileset loaded";

  public void Enter()
  {
    context.View.Workspace.TileProbability.Show();
    context.Redrawer.RedrawProbabilityLabels();
    selectedProbabilityTile = context.Model.ImagePath == "" ? null : Vector2I.Zero;
    SyncSpinBox();
    RedrawSelection();
  }

  public void Exit()
  {
    context.View.Workspace.TileProbability.Hide();
    selectedProbabilityTile = null;
    context.View.Workspace.ClearProbabilitySelection();
    SyncSpinBox();
  }

  public void HandleMouseInput(InputEventMouse inputEventMouse, Vector2 position)
  {
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;
    if (mouseLeftClicked)
      SelectProbabilityTile(new Vector2I((int)position.X, (int)position.Y));
  }

  public void HandleMouseMotion(Vector2 position)
    => context.Redrawer.DrawTileHover(position);

  public bool HandleMouseWheel(int delta, bool shiftPressed)
  {
    if (!HasSelection)
      return false;

    int step = shiftPressed ? 10 : 1;
    AdjustSelectedProbability(delta * step);
    return true;
  }

  private void SelectProbabilityTile(Vector2I worldPosition)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, context.Redrawer.ScaledTileSize);
    var bitmaskData = context.Model.BitmaskDatabase.GetBitmaskData(context.Model.ImagePath, scaledTilePosition);
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

    long currentValue = bitmaskData.GetProbability(context.Model.CurrentLayer);
    SetSelectedProbability((uint)Math.Clamp(currentValue + delta, 0L, uint.MaxValue));
  }

  private void SetSelectedProbability(uint value)
  {
    if (!TryGetSelectedBitmaskData(out var bitmaskData))
      return;

    bitmaskData.SetProbability(context.Model.CurrentLayer, value);
    context.View.Workspace.TileProbability.ChangeLabelProbability(selectedProbabilityTile!.Value, value);
    context.View.ToolsPanel.ProbabilityPanel.SetProbabilityValue(value);
  }

  private bool TryGetSelectedBitmaskData(out BitmaskData bitmaskData)
  {
    bitmaskData = null!;
    if (selectedProbabilityTile is null)
      return false;

    var result = context.Model.BitmaskDatabase.GetBitmaskData(context.Model.ImagePath, selectedProbabilityTile.Value);
    if (result is null || result.IsEmpty())
      return false;

    bitmaskData = result;
    return true;
  }

  public void SyncSpinBox()
  {
    context.View.ToolsPanel.ProbabilityPanel.SetSelectedPosition(selectedProbabilityTile);
    bool hasData = TryGetSelectedBitmaskData(out var bitmaskData);
    context.View.ToolsPanel.ProbabilityPanel.SetProbabilityEnabled(hasData);
    if (hasData)
      context.View.ToolsPanel.ProbabilityPanel.SetProbabilityValue(bitmaskData.GetProbability(context.Model.CurrentLayer));
  }

  public void RedrawSelection()
  {
    if (selectedProbabilityTile is null)
    {
      context.View.Workspace.ClearProbabilitySelection();
      return;
    }

    context.View.Workspace.RedrawProbabilitySelection(
      selectedProbabilityTile.Value * context.Redrawer.ScaledTileSize, context.Settings.EditorSettings.SelectionColor, context.Redrawer.ScaledTileSize);
  }

  public void ResetSelection()
  {
    selectedProbabilityTile = null;
    context.View.Workspace.ClearProbabilitySelection();
    SyncSpinBox();
  }
}
