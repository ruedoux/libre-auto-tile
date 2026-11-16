using Godot;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;
using LibreAutoTile.Tiling.Search.Models;

namespace LibreAutoTile.GUI.Editor.Controller;

public class EditorRedrawer(EditorModel model, EditorView view, AppSettings settings)
{
  private readonly EditorModel model = model;
  private readonly EditorView view = view;
  private readonly AppSettings settings = settings;

  public int ScaledTileSize => model.TileSize * settings.WorldSettings.ImageScaling;

  public void RefreshTilesView()
    => view.ToolsPanel.TilesPanel.Render(model.Tiles.Tiles, model.Tiles.ActiveTile);

  public void RefreshImageOptions()
    => view.ToolsPanel.TilesPanel.RefreshImageOptions(
      model.BitmaskDatabase.GetAll().Keys.OrderBy(x => x), model.ImagePath);

  public void RedrawGrid()
  {
    if (model.ImageSize == Vector2I.Zero)
      return;
    view.Workspace.RedrawGrid(new(Vector2I.Zero, model.ImageSize), settings.EditorSettings.GridColor, ScaledTileSize);
  }

  public void RedrawAll()
  {
    RedrawGrid();
    RedrawProbabilityLabels();
    RedrawBitmask();
  }

  public void DrawTileHover(Vector2 position)
  {
    var snappedTilePosition = TileSetMath.SnapToTileCorner(position, ScaledTileSize);
    view.Workspace.RedrawTile(snappedTilePosition, settings.EditorSettings.SelectionColor, ScaledTileSize, model.TileShape);
    var tilePosition = TileSetMath.ScaleDownTilePosition(position, ScaledTileSize);
    view.Workspace.DisplayTileLabel(tilePosition.ToString());
  }

  public bool HasAnyBitmaskCentre()
  {
    foreach (var (_, positionToBitmaskData) in model.BitmaskDatabase.GetAll())
      foreach (var (_, bitmaskData) in positionToBitmaskData)
        foreach (var layer in bitmaskData.GetLayers())
          if (bitmaskData.GetCentreTileId(layer) >= 0)
            return true;
    return false;
  }

  public void RedrawBitmask()
  {
    if (ScaledTileSize < 1)
      throw new InvalidOperationException("Tile size cannot be less than 1");

    var tileIdToColor = model.Tiles.Tiles.ToDictionary(x => x.TileId, x => x.Color);

    Dictionary<Rect2I, Color> bitmaskRectanglesToColors = [];
    foreach (var (scaledTilePosition, bitmaskData) in model.BitmaskDatabase.GetAllByFileName(model.ImagePath))
    {
      var snappedTilePosition = scaledTilePosition * ScaledTileSize;
      var centreTileId = bitmaskData.GetCentreTileId(model.CurrentLayer);
      var tileMask = bitmaskData.GetTileMask(model.CurrentLayer);

      if (centreTileId >= 0)
      {
        if (!tileIdToColor.TryGetValue(centreTileId, out var color))
          throw new InvalidOperationException($"Centre tile id '{centreTileId}' is not mapped to any color");

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
          throw new InvalidOperationException($"Tile mask tile id '{tileId}' is not mapped to any color");

        var bitmaskPosition = TileSetMath.DirectionToPosition(
          (TileMask.SurroundingDirection)i);
        Rect2I bitmaskRectangle = TileSetMath.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, bitmaskPosition, ScaledTileSize);
        bitmaskRectanglesToColors[bitmaskRectangle] = color;
      }
    }

    view.Workspace.BitmaskDrawer.RedrawBitmask(bitmaskRectanglesToColors);
  }

  public void RedrawProbabilityLabels()
  {
    view.Workspace.TileProbability.Clear();

    int endX = model.ImageSize.X;
    int endY = model.ImageSize.Y;
    for (int x = 0; x < endX; x += ScaledTileSize)
    {
      for (int y = 0; y < endY; y += ScaledTileSize)
      {
        var scaledTilePosition = TileSetMath.ScaleDownTilePosition(new Vector2(x, y), ScaledTileSize);
        var bitmaskData = model.BitmaskDatabase.GetBitmaskData(model.ImagePath, scaledTilePosition);
        uint probability = bitmaskData is null ? 1 : bitmaskData.GetProbability(model.CurrentLayer);
        view.Workspace.TileProbability.AddLabel(scaledTilePosition, probability, ScaledTileSize);
      }
    }
  }
}
