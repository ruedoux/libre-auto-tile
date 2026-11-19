using Godot;
using LibreAutoTile.Configuration;
using LibreAutoTile.GUI.Editor.Model;
using TileShape = LibreAutoTile.Configuration.Models.TileShape;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public class PreviewToolController(EditorContext context) : IToolState
{
  private readonly EditorContext context = context;

  public bool CanActivate() => context.Redrawer.HasAnyBitmaskCentre();
  public string ActivationError => "No bitmask data to preview";
  public bool ShowsMouseLabel => false;

  public void Enter()
  {
    context.View.HideWorkspace();
    context.View.Workspace.SetInfiniteCameraView();
    context.View.Workspace.ShowPreviewHighlight();

    AutoTileConfiguration autoTileConfiguration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      context.Model.Tiles.Tiles,
      context.Model.BitmaskDatabase,
      context.Model.ImageRootPath,
      context.Model.TileSize,
      context.Model.TileShape,
      context.Model.WildcardId);
    context.View.ToolsPanel.PreviewPanel.InitializeTileMap(autoTileConfiguration, context.Model.TileShape);
    context.View.ToolsPanel.PreviewPanel.AddCreatedTiles(
      [.. context.Model.Tiles.Tiles.Select(t => (t.TileId, t.TileName))],
      autoTileConfiguration);

    var autoTileMap = context.View.ToolsPanel.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
    {
      context.View.Workspace.AddChild(autoTileMap);
      autoTileMap.Scale = new(context.Settings.WorldSettings.ImageScaling, context.Settings.WorldSettings.ImageScaling);
    }

    context.Logger.Log("Entered preview and loaded AutoTileMap");
  }

  public void Exit()
  {
    context.View.ShowWorkspace();
    context.View.Workspace.HidePreviewHighlight();
    context.View.Workspace.SetCameraView(new(Vector2I.Zero, context.Model.ImageSize));

    var autoTileMap = context.View.ToolsPanel.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
      context.View.Workspace.RemoveChild(autoTileMap);

    context.Redrawer.RedrawGrid();
    context.Redrawer.RedrawBitmask();
    context.Logger.Log("Exited preview and unloaded AutoTileMap");
  }

  public void HandleMouseInput(InputEventMouse inputEventMouse, Vector2 position)
  {
    var autoTileMap = context.View.ToolsPanel.PreviewPanel.AutoTileMap;
    var activeTile = context.View.ToolsPanel.PreviewPanel.ActiveTile;
    if (autoTileMap is null || activeTile is null)
      return;

    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;

    if (mouseLeftClicked)
      autoTileMap.DrawTiles(
        0, [new(autoTileMap.WorldToMap(position / context.Settings.WorldSettings.ImageScaling), activeTile.TileId)]);
    if (mouseRightClicked)
      autoTileMap.DrawTiles(
        0, [new(autoTileMap.WorldToMap(position / context.Settings.WorldSettings.ImageScaling), -1)]);
  }

  public void HandleMouseMotion(Vector2 position)
  {
    var autoTileMap = context.View.ToolsPanel.PreviewPanel.AutoTileMap;
    if (autoTileMap is null)
      return;

    var mapPosition = autoTileMap.WorldToMap(position / context.Settings.WorldSettings.ImageScaling);
    // MapToLocal returns the center of the cell, convert to the tiles bounding box top left
    // The isometric diamond bounding box is tileSize wide and tileSize/2 tall
    Vector2 center = autoTileMap.GetTileMapLayer(0).MapToLocal(mapPosition) * context.Settings.WorldSettings.ImageScaling;
    Vector2 tileTopLeft = context.Model.TileShape == TileShape.Isometric
      ? center - new Vector2(context.Redrawer.ScaledTileSize / 2f, context.Redrawer.ScaledTileSize / 4f)
      : center - new Vector2(context.Redrawer.ScaledTileSize / 2f, context.Redrawer.ScaledTileSize / 2f);

    context.View.Workspace.RedrawPreviewHighlight(
      tileTopLeft, context.Settings.EditorSettings.SelectionColor, context.Redrawer.ScaledTileSize, context.Model.TileShape);
  }
}
