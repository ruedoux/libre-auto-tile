using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class PreviewController
{
  private readonly EditorContext context;

  public PreviewController(EditorContext context)
  {
    this.context = context;
  }

  public void EnterPreview()
  {
    context.EditorScene.HideWorkspace();
    context.EditorScene.SetInfiniteCameraView();
    context.EditorScene.ShowPreviewHighlight();

    AutoTileConfiguration autoTileConfiguration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      context.EditorData.Tiles.Tiles, context.EditorData.BitmaskDatabase, context.EditorData.TileSize, context.EditorData.TileShape,
      context.EditorData.WildcardId);
    context.EditorScene.PreviewPanel.InitializeTileMap(autoTileConfiguration, context.EditorData.TileShape);
    context.EditorScene.PreviewPanel.AddCreatedTiles(
      [.. context.EditorData.Tiles.Tiles.Select(t => (t.TileId, t.TileName))],
      autoTileConfiguration);

    var autoTileMap = context.EditorScene.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
    {
      context.EditorScene.AddChild(autoTileMap);
      autoTileMap.Scale = new(Settings.IMAGE_SCALING, Settings.IMAGE_SCALING);
    }

    GodotLogger.LOGGER.Log("Entered preview and loaded AutoTileMap");
  }

  public void ExitPreview()
  {
    context.EditorScene.ShowWorkspace();
    context.EditorScene.HidePreviewHighlight();
    context.EditorScene.SetCameraView(new(Vector2I.Zero, context.EditorData.ImageSize));

    var autoTileMap = context.EditorScene.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
      context.EditorScene.RemoveChild(autoTileMap);

    context.RedrawGrid();
    context.RedrawBitmask();
    GodotLogger.LOGGER.Log("Exited preview and unloaded AutoTileMap");
  }

  public void HandleMouseInput(InputEventMouse inputEventMouse)
  {
    if (context.ActiveTool != EditorTool.Preview)
      return;
    if (GodotExtensions.IsMouseOnElements(context.UiElements))
      return;

    var autoTileMap = context.EditorScene.PreviewPanel.AutoTileMap;
    var activeTile = context.EditorScene.PreviewPanel.ActiveTile;
    if (autoTileMap is null || activeTile is null)
      return;

    var mousePosition = context.EditorScene.GetGlobalMousePosition();
    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;

    if (mouseLeftClicked)
      autoTileMap.DrawTiles(
        0, [new(autoTileMap.WorldToMap(mousePosition / Settings.IMAGE_SCALING), activeTile.TileId)]);
    if (mouseRightClicked)
      autoTileMap.DrawTiles(
        0, [new(autoTileMap.WorldToMap(mousePosition / Settings.IMAGE_SCALING), -1)]);

    if (mouseLeftClicked || mouseRightClicked)
    {
      List<Vector2I> surroundingPositions = [];
      var mapPosition = autoTileMap.WorldToMap(mousePosition / Settings.IMAGE_SCALING);
      for (int x = -1; x < 2; x++)
        for (int y = -1; y < 2; y++)
          surroundingPositions.Add(mapPosition + new Vector2I(x, y));
      autoTileMap.UpdateTiles(0, [.. surroundingPositions]);
    }
  }
}
