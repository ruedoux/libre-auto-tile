using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views.Presentation;
using Qwaitumin.LibreAutoTile.GUI.Views;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

/// <summary>
/// Single facade controllers use to read the model and trigger view rendering.
/// Views only ever receive primitives/DTOs through this facade, never the model directly.
/// </summary>
public class EditorContext
{
  // Model access
  public readonly EditorData EditorData;
  public readonly AppearanceSettings AppearanceSettings = new();

  // View access
  public readonly EditorScene EditorScene;
  public readonly Control[] UiElements;

  public EditorTool ActiveTool;
  public ProbabilityController Probability { get; set; } = null!;

  public int ScaledTileSize => EditorData.TileSize * Settings.IMAGE_SCALING;

  public EditorContext(EditorData editorData, EditorScene editorScene)
  {
    EditorData = editorData;
    EditorScene = editorScene;

    UiElements =
    [
      editorScene.OptionToolsTabs, editorScene.SelectImageButton, editorScene.SaveButton,
      editorScene.LoadButton, editorScene.ClearButton, editorScene.LayerControl
    ];
  }

  // Render orchestration
  public void RefreshTilesView()
    => EditorScene.TilesPanel.Render(TileViewMapper.ToViewModels(EditorData.Tiles));

  public void RefreshImageOptions()
    => EditorScene.TilesPanel.RefreshImageOptions(
      EditorData.BitmaskDatabase.GetAll().Keys.OrderBy(x => x), EditorData.ImagePath);

  public void RedrawGrid()
  {
    if (EditorData.ImageSize == Vector2I.Zero)
      return;
    EditorScene.RedrawGrid(new(Vector2I.Zero, EditorData.ImageSize), AppearanceSettings.GridColor, ScaledTileSize);
  }

  public bool HasAnyBitmaskCentre()
  {
    foreach (var (_, positionToBitmaskData) in EditorData.BitmaskDatabase.GetAll())
      foreach (var (_, bitmaskData) in positionToBitmaskData)
        foreach (var layer in bitmaskData.GetLayers())
          if (bitmaskData.GetCentreTileId(layer) >= 0)
            return true;
    return false;
  }

  public void RedrawBitmask()
  {
    if (ScaledTileSize < 1)
      GodotLogger.LogErrorAndThrow("Tile size cannot be less than 1");

    var tileIdToColor = EditorData.Tiles.Tiles.ToDictionary(x => x.TileId, x => x.Color);

    Dictionary<Rect2I, Color> bitmaskRectanglesToColors = [];
    foreach (var (scaledTilePosition, bitmaskData) in EditorData.BitmaskDatabase.GetAllByFileName(EditorData.ImagePath))
    {
      var snappedTilePosition = scaledTilePosition * ScaledTileSize;
      var centreTileId = bitmaskData.GetCentreTileId(EditorData.CurrentLayer);
      var tileMask = bitmaskData.GetTileMask(EditorData.CurrentLayer);

      if (centreTileId >= 0)
      {
        if (!tileIdToColor.TryGetValue(centreTileId, out var color))
          GodotLogger.LogErrorAndThrow($"Centre tile id '{centreTileId}' is not mapped to any color");

        Rect2I centreRectangle = TileSetMath.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, TileSetMath.MIDDLE, ScaledTileSize);
        bitmaskRectanglesToColors[centreRectangle] = color;
      }

      var tileMaskArray = tileMask.ToArray();
      for (int i = 0; i < tileMaskArray.Length; i++)
      {
        var tileId = tileMaskArray[i];
        if (tileId < 0)
          continue;

        if (!tileIdToColor.TryGetValue(tileId, out var color))
          GodotLogger.LogErrorAndThrow($"Tile mask tile id '{tileId}' is not mapped to any color");

        var bitmaskPosition = TileSetMath.DirectionToPosition(
          (TileMask.SurroundingDirection)i);
        Rect2I bitmaskRectangle = TileSetMath.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, bitmaskPosition, ScaledTileSize);
        bitmaskRectanglesToColors[bitmaskRectangle] = color;
      }
    }

    EditorScene.BitmaskDrawer.RedrawBitmask(bitmaskRectanglesToColors);
  }

  public void RedrawProbabilityLabels()
  {
    EditorScene.TileProbability.Clear();

    int endX = EditorData.ImageSize.X;
    int endY = EditorData.ImageSize.Y;
    for (int x = 0; x < endX; x += ScaledTileSize)
    {
      for (int y = 0; y < endY; y += ScaledTileSize)
      {
        var scaledTilePosition = TileSetMath.ScaleDownTilePosition(new Vector2(x, y), ScaledTileSize);
        var bitmaskData = EditorData.BitmaskDatabase.GetBitmaskData(EditorData.ImagePath, scaledTilePosition);
        uint probability = bitmaskData is null ? 1 : bitmaskData.GetProbability(EditorData.CurrentLayer);
        EditorScene.TileProbability.AddLabel(scaledTilePosition, probability, ScaledTileSize);
      }
    }
  }
}
