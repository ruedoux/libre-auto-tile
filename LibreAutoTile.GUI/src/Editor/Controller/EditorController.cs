using LibreAutoTile.GUI.Editor.Controller.Tools;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.Controller;

public class EditorController
{
  private readonly EditorModel model;
  private readonly EditorView view;
  private readonly EditorRedrawer redrawer;
  private readonly ToolStateMachine toolStateMachine;
  private readonly ProbabilityToolController probabilityTool;

  public EditorController(EditorModel model, EditorView view, AppSettings settings, GuiLogger logger)
  {
    this.model = model;
    this.view = view;

    redrawer = new EditorRedrawer(model, view, settings);
    var context = new EditorContext(model, view, settings, logger, redrawer);
    toolStateMachine = new ToolStateMachine(context);

    var tilesTool = toolStateMachine.TilesTool;
    probabilityTool = toolStateMachine.ProbabilityTool;

    var tileController = new TileController(context);
    _ = new ConfigurationController(context);
    _ = new SettingsController(context);
    _ = new InputController(context, toolStateMachine);

    view.LayerControl.SetLayer(model.CurrentLayer);

    if (model.Tiles.Tiles.Count == 0)
    {
      model.Tiles.AddNew();
      model.Tiles.SetActive(model.Tiles.Tiles[0]);
    }
    redrawer.RefreshTilesView();

    tileController.TileDeleted += OnTileDeleted;
    tileController.TileIdChanged += OnTileIdChanged;
    tilesTool.LayerChanged += OnLayerChanged;
  }

  private void OnTileDeleted(int tileId)
  {
    foreach (var position in model.BitmaskDatabase.GetPositionsWithCentreTileId(tileId))
      view.Workspace.TileProbability.ChangeLabelProbability(position, 1);

    model.BitmaskDatabase.RemapTileIdEverywhere(tileId, -1);
    redrawer.RedrawBitmask();
    probabilityTool.SyncSpinBox();
  }

  private void OnTileIdChanged(int newId, int oldId)
  {
    model.BitmaskDatabase.RemapTileIdEverywhere(oldId, newId);
    redrawer.RedrawBitmask();
  }

  private void OnLayerChanged(int layer)
  {
    model.CurrentLayer = layer;
    view.LayerControl.SetLayer(layer);
    redrawer.RedrawBitmask();
    redrawer.RedrawProbabilityLabels();
    probabilityTool.SyncSpinBox();
  }
}
