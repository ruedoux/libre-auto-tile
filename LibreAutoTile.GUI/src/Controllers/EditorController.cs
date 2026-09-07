using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;
using TileShape = Qwaitumin.LibreAutoTile.Configuration.Models.TileShape;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class EditorController
{
  private const string TILES_TAB_NAME = "Tiles";
  private const string PREVIEW_TAB_NAME = "Preview";
  private const string PROBABILITY_TAB_NAME = "Chance";

  private readonly EditorContext context;
  private readonly ProbabilityController probabilityController;
  private readonly PreviewController previewController;

  private int previousTabIndex;

  public EditorController(EditorData data, EditorScene view)
  {
    context = new EditorContext(data, view);

    previousTabIndex = view.OptionToolsTabs.CurrentTab;
    context.ActiveTool = GetToolForTab(view.OptionToolsTabs.CurrentTab);

    SettingsController.SeedViewFromModel(context);
    view.LayerControl.SetLayer(data.CurrentLayer);

    if (data.Tiles.Tiles.Count == 0)
    {
      data.Tiles.AddNew();
      data.Tiles.SetActive(data.Tiles.Tiles[0]);
    }
    context.RefreshTilesView();
    probabilityController = new ProbabilityController(context);
    context.Probability = probabilityController;
    var tileController = new TileController(context);
    var configurationController = new ConfigurationController(context);
    var settingsController = new SettingsController(context);
    var bitmaskController = new BitmaskController(context);
    previewController = new PreviewController(context);

    view.OptionToolsTabs.TabChanged += OnTabChanged;
    tileController.TileDeleted += OnTileDeleted;
    tileController.TileIdChanged += OnTileIdChanged;
    bitmaskController.LayerChanged += SetLayer;

    view.InputListener.AddInputAction(_ =>
    {
      bool isMouseOnUi = GodotExtensions.IsMouseOnElements(context.UiElements);
      view.SetMouseLabelVisible(!isMouseOnUi && context.ActiveTool != EditorTool.Preview);
      if (isMouseOnUi)
        view.BitmaskDrawer.HideBitmaskGhost();
      else
        view.BitmaskDrawer.ShowBitmaskGhost();
    });

    view.InputListener.AddInputMouseMotionAction(mouseMotion =>
    {
      if (mouseMotion.ButtonMask == MouseButtonMask.Middle)
        view.CameraControl.MoveCamera(-mouseMotion.Relative);
      else if (!GodotExtensions.IsMouseOnElements(context.UiElements))
        OnMouseMoved(view.GetGlobalMousePosition());
    });

    view.InputListener.AddInputMouseButtonAction(HandleMouseWheel);

    view.InputListener.AddInputMouseButtonAction(bitmaskController.HandleMouseInput);
    view.InputListener.AddInputMouseMotionAction(bitmaskController.HandleMouseInput);
    view.InputListener.AddInputMouseButtonAction(probabilityController.HandleMouseInput);
    view.InputListener.AddInputMouseMotionAction(probabilityController.HandleMouseInput);
    view.InputListener.AddInputMouseButtonAction(previewController.HandleMouseInput);
    view.InputListener.AddInputMouseMotionAction(previewController.HandleMouseInput);
  }

  private void OnMouseMoved(Vector2 mousePosition)
  {
    if (context.ActiveTool == EditorTool.Preview)
    {
      RedrawPreviewHighlight(mousePosition);
      return;
    }

    var snappedTilePosition = TileSetMath.SnapToTileCorner(mousePosition, context.ScaledTileSize);
    context.EditorScene.RedrawTile(snappedTilePosition, context.AppearanceSettings.SelectionColor, context.ScaledTileSize, context.EditorData.TileShape);
    var tilePosition = TileSetMath.ScaleDownTilePosition(mousePosition, context.ScaledTileSize);
    context.EditorScene.DisplayTileLabel(tilePosition.ToString());

    if (context.ActiveTool == EditorTool.Tiles)
      RedrawBitmaskGhost(mousePosition);
  }

  private void RedrawPreviewHighlight(Vector2 mousePosition)
  {
    var autoTileMap = context.EditorScene.PreviewPanel.AutoTileMap;
    if (autoTileMap is null)
      return;

    var mapPosition = autoTileMap.WorldToMap(mousePosition / Settings.IMAGE_SCALING);
    // MapToLocal returns the center of the cell; convert to the tile's bounding-box top-left.
    // The isometric diamond bounding box is tileSize wide and tileSize/2 tall.
    Vector2 center = autoTileMap.GetTileMapLayer(0).MapToLocal(mapPosition) * Settings.IMAGE_SCALING;
    Vector2 tileTopLeft = context.EditorData.TileShape == TileShape.Isometric
      ? center - new Vector2(context.ScaledTileSize / 2f, context.ScaledTileSize / 4f)
      : center - new Vector2(context.ScaledTileSize / 2f, context.ScaledTileSize / 2f);

    context.EditorScene.RedrawPreviewHighlight(
      tileTopLeft, context.AppearanceSettings.SelectionColor, context.ScaledTileSize, context.EditorData.TileShape);
  }

  private void RedrawBitmaskGhost(Vector2 worldPosition)
    => context.EditorScene.BitmaskDrawer.RedrawBitmaskGhost(
      worldPosition, context.ScaledTileSize, new(r: 255f, g: 255f, b: 255f, a: 0.5f));

  private void OnTabChanged(long tab)
  {
    int newTab = (int)tab;

    if (IsTab(newTab, PREVIEW_TAB_NAME) && !context.HasAnyBitmaskCentre())
    {
      context.EditorScene.MessageDisplay.DisplayText("[color=red]No bitmask data to preview[/color]");
      context.EditorScene.OptionToolsTabs.CurrentTab = previousTabIndex;
      return;
    }

    if (IsTab(newTab, PROBABILITY_TAB_NAME) && context.EditorData.ImagePath == "")
    {
      context.EditorScene.MessageDisplay.DisplayText("[color=red]No tileset loaded[/color]");
      context.EditorScene.OptionToolsTabs.CurrentTab = previousTabIndex;
      return;
    }

    previousTabIndex = newTab;

    EditorTool newTool = GetToolForTab(newTab);

    if (newTool == EditorTool.Preview && context.ActiveTool != EditorTool.Preview)
      previewController.EnterPreview();
    else if (newTool != EditorTool.Preview && context.ActiveTool == EditorTool.Preview)
      previewController.ExitPreview();

    if (newTool == EditorTool.Probability && context.ActiveTool != EditorTool.Probability)
      probabilityController.EnterProbability();
    else if (newTool != EditorTool.Probability && context.ActiveTool == EditorTool.Probability)
      probabilityController.ExitProbability();

    context.ActiveTool = newTool;
  }

  private EditorTool GetToolForTab(int tabIndex)
  {
    if (IsTab(tabIndex, PREVIEW_TAB_NAME))
      return EditorTool.Preview;
    if (IsTab(tabIndex, PROBABILITY_TAB_NAME))
      return EditorTool.Probability;
    if (IsTab(tabIndex, TILES_TAB_NAME))
      return EditorTool.Tiles;
    return EditorTool.None;
  }

  private bool IsTab(int tabIndex, string name)
    => context.EditorScene.OptionToolsTabs.GetTabTitle(tabIndex) == name;

  private void SetLayer(int layer)
  {
    context.EditorData.CurrentLayer = layer;
    context.EditorScene.LayerControl.SetLayer(layer);
    context.RedrawBitmask();
    context.RedrawProbabilityLabels();
    probabilityController.SyncSpinBox();
  }

  private void HandleMouseWheel(InputEventMouseButton mouseButton)
  {
    int delta = 0;
    if (mouseButton.ButtonIndex == MouseButton.WheelDown)
      delta = -1;
    else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
      delta = 1;
    else
      return;

    if (context.ActiveTool == EditorTool.Probability && probabilityController.HasSelection
        && !GodotExtensions.IsMouseOnElements(context.UiElements))
    {
      int step = mouseButton.ShiftPressed ? 10 : 1;
      probabilityController.AdjustSelectedProbability(delta * step);
      return;
    }

    context.EditorScene.CameraControl.ZoomCamera(delta * context.EditorScene.CameraControl.ZoomValue);
  }

  private void OnTileDeleted(int tileId)
  {
    foreach (var (_, positionToPackedTileData) in context.EditorData.BitmaskDatabase.GetAll())
    {
      foreach (var (position, bitmaskData) in positionToPackedTileData)
      {
        foreach (var layer in bitmaskData.GetLayers())
          if (bitmaskData.GetCentreTileId(layer) == tileId)
            context.EditorScene.TileProbability.ChangeLabelProbability(position, 1);

        bitmaskData.RemoveTileId(tileId);
      }
    }
    context.RedrawBitmask();
    probabilityController.SyncSpinBox();
  }

  private void OnTileIdChanged(int newId, int oldId)
  {
    foreach (var (_, positionToPackedTileData) in context.EditorData.BitmaskDatabase.GetAll())
      foreach (var (_, bitmaskData) in positionToPackedTileData)
        bitmaskData.ChangeTileId(newId, oldId);
    context.RedrawBitmask();
  }
}
