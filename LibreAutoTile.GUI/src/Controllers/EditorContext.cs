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
  public readonly EditorData Data;
  public readonly AppearanceSettings Appearance = new();

  // View access
  public readonly EditorScene View;
  public readonly Control[] UiElements;

  public EditorTool ActiveTool;
  public ProbabilityController Probability { get; set; } = null!;

  public int ScaledTileSize => Data.TileSize * Settings.IMAGE_SCALING;

  public EditorContext(EditorData data, EditorScene view)
  {
    Data = data;
    View = view;

    UiElements =
    [
      view.OptionToolsTabs, view.SelectImageButton, view.SaveButton,
      view.LoadButton, view.ClearButton, view.LayerControl
    ];
  }

  // Render orchestration
  public void RefreshTilesView()
    => View.TilesPanel.Render(TileViewMapper.ToViewModels(Data.Tiles));

  public void RefreshImageOptions()
    => View.TilesPanel.RefreshImageOptions(
      Data.BitmaskDatabase.GetAll().Keys.OrderBy(x => x), Data.ImagePath);

  public void RedrawGrid()
  {
    if (Data.ImageSize == Vector2I.Zero)
      return;
    View.RedrawGrid(new(Vector2I.Zero, Data.ImageSize), Appearance.GridColor, ScaledTileSize);
  }

  public bool HasAnyBitmaskCentre()
  {
    foreach (var (_, positionToBitmaskData) in Data.BitmaskDatabase.GetAll())
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

    var tileIdToColor = Data.Tiles.Tiles.ToDictionary(x => x.TileId, x => x.Color);

    Dictionary<Rect2I, Color> bitmaskRectanglesToColors = [];
    foreach (var (scaledTilePosition, bitmaskData) in Data.BitmaskDatabase.GetAllByFileName(Data.ImagePath))
    {
      var snappedTilePosition = scaledTilePosition * ScaledTileSize;
      var centreTileId = bitmaskData.GetCentreTileId(Data.CurrentLayer);
      var tileMask = bitmaskData.GetTileMask(Data.CurrentLayer);

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

    View.BitmaskDrawer.RedrawBitmask(bitmaskRectanglesToColors);
  }

  public void RedrawProbabilityLabels()
  {
    View.TileProbability.Clear();

    int endX = Data.ImageSize.X;
    int endY = Data.ImageSize.Y;
    for (int x = 0; x < endX; x += ScaledTileSize)
    {
      for (int y = 0; y < endY; y += ScaledTileSize)
      {
        var scaledTilePosition = TileSetMath.ScaleDownTilePosition(new Vector2(x, y), ScaledTileSize);
        var bitmaskData = Data.BitmaskDatabase.GetBitmaskData(Data.ImagePath, scaledTilePosition);
        uint probability = bitmaskData is null ? 1 : bitmaskData.GetProbability(Data.CurrentLayer);
        View.TileProbability.AddLabel(scaledTilePosition, probability, ScaledTileSize);
      }
    }
  }
}
