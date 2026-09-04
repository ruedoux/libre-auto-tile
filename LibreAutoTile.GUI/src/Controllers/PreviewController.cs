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
    context.View.HideWorkspace();
    context.View.SetInfiniteCameraView();
    context.View.ShowPreviewHighlight();

    AutoTileConfiguration autoTileConfiguration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      context.Data.Tiles.Tiles, context.Data.BitmaskDatabase, context.Data.TileSize, context.Data.TileShape);
    context.View.PreviewPanel.InitializeTileMap(autoTileConfiguration, context.Data.TileShape);
    context.View.PreviewPanel.AddCreatedTiles(
      [.. context.Data.Tiles.Tiles.Select(t => (t.TileId, t.TileName))],
      autoTileConfiguration);

    var autoTileMap = context.View.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
    {
      context.View.AddChild(autoTileMap);
      autoTileMap.Scale = new(Settings.IMAGE_SCALING, Settings.IMAGE_SCALING);
    }

    GodotLogger.LOGGER.Log("Entered preview and loaded AutoTileMap");
  }

  public void ExitPreview()
  {
    context.View.ShowWorkspace();
    context.View.HidePreviewHighlight();
    context.View.SetCameraView(new(Vector2I.Zero, context.Data.ImageSize));

    var autoTileMap = context.View.PreviewPanel.AutoTileMap;
    if (autoTileMap is not null)
      context.View.RemoveChild(autoTileMap);

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

    var autoTileMap = context.View.PreviewPanel.AutoTileMap;
    var activeTile = context.View.PreviewPanel.ActiveTile;
    if (autoTileMap is null || activeTile is null)
      return;

    var mousePosition = context.View.GetGlobalMousePosition();
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
