using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.GUI.Core.GodotBindings;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.Logging;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;

public partial class BitmaskContainer : Node2D
{
  public TextureRect TileSetTexture { private set; get; } = null!;
  public readonly TileSetBitmaskDrawer TileSetBitmaskDrawer;
  public TileDatabase TileDatabase { private set; get; } = new();

  public BitmaskContainer()
  {
    TileSetBitmaskDrawer = GodotApi.AddChild<TileSetBitmaskDrawer>(this, new());
  }

  public override void _Ready()
  {
    TileSetTexture = GodotApi.AddChild<TextureRect>(this, new());
    TileSetTexture.TextureFilter = TextureFilterEnum.Nearest;
  }

  public void Clear()
  {
    TileDatabase.Clear();
    TileSetBitmaskDrawer.RedrawBitmask([]);
    GodotLogger.Logger.Log("Cleared all Bitmasks");
  }

  public void RedrawBitmask(
    string filePath,
    int layer,
    Dictionary<int, string> tileIdToTileNames,
    Dictionary<string, Color> existingTileNamesToColors,
    int tileSize)
  {
    if (tileSize < 1) throw new ArgumentException("Tile size cannot be less than 1");

    Dictionary<Rect2I, Color> bitmaskRectanglesToColors = [];
    foreach (var (scaledTilePosition, packedTileData) in TileDatabase.GetAllByFileName(filePath))
    {
      var snappedTilePosition = scaledTilePosition * tileSize;
      var centreTileId = packedTileData.GetCentreTileId(layer);
      var tileMask = packedTileData.GetTileMask(layer);

      if (centreTileId >= 0)
      {
        if (!tileIdToTileNames.TryGetValue(centreTileId, out var centreTileName))
          throw new ArgumentException($"Centre tile id '{centreTileId}' is not mapped to any name");
        if (!existingTileNamesToColors.TryGetValue(centreTileName, out var color))
          throw new ArgumentException($"Tile name '{centreTileName}' is not mapped to any color");

        Rect2I centreRectangle = BitmaskCalculator.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, BitmaskCalculator.MIDDLE, tileSize);
        bitmaskRectanglesToColors[centreRectangle] = color;
      }

      var tileMaskArray = tileMask.ToArray();
      for (int i = 0; i < tileMaskArray.Length; i++)
      {
        var tileId = tileMaskArray[i];
        if (tileId < 0)
          continue;

        if (!tileIdToTileNames.TryGetValue(tileId, out var tileName))
          throw new ArgumentException($"Tile mask tile id '{tileId}' is not mapped to any name");
        if (!existingTileNamesToColors.TryGetValue(tileName, out var color))
          throw new ArgumentException($"Tile name '{tileName}' is not mapped to any color");

        var bitmaskPosition = BitmaskCalculator.DirectionToPosition(
          (TileMask.SurroundingDirection)i);
        Rect2I bitmaskRectangle = BitmaskCalculator.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, bitmaskPosition, tileSize);
        bitmaskRectanglesToColors[bitmaskRectangle] = color;
      }
    }

    TileSetBitmaskDrawer.RedrawBitmask(bitmaskRectanglesToColors);
  }

  public void RedrawBitmaskGhost(Vector2 worldPosition, int tileSize, Color color)
    => TileSetBitmaskDrawer.RedrawBitmaskGhost(worldPosition, tileSize, color);

  public void PlaceBitmask(
    int layer, int tileId, string fileName, Vector2 worldPosition, int tileSize)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, tileSize);
    var packedTileData = TileDatabase.GetPackedTileData(fileName, scaledTilePosition);
    var bitmaskPosition = BitmaskCalculator.DetermineBitmask(worldPosition, tileSize);

    packedTileData.AddBitmask(layer, tileId, bitmaskPosition);
  }

  public void RemoveBitmask(int layer, string fileName, Vector2 worldPosition, int tileSize)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, tileSize);
    var packedTileData = TileDatabase.GetPackedTileData(fileName, scaledTilePosition);
    var bitmaskPosition = BitmaskCalculator.DetermineBitmask(worldPosition, tileSize);
    packedTileData.RemoveBitmask(layer, bitmaskPosition);

    if (packedTileData.LayerFullTileMask.Count == 0)
      TileDatabase.GetAllByFileName(fileName).Remove(scaledTilePosition);
  }

  public void RemoveTileId(int tileId)
  {
    foreach (var (_, positionToPackedTileData) in TileDatabase.GetAll())
    {
      foreach (var (_, packedTileData) in positionToPackedTileData)
      {
        foreach (var (layer, fullTileMask) in packedTileData.LayerFullTileMask)
        {
          var centreTileId = packedTileData.GetCentreTileId(layer);
          if (centreTileId == tileId)
            packedTileData.SetCentreTileId(layer, -1);

          var tileMask = packedTileData.GetTileMask(layer);
          var updatedTileMaskArray = tileMask.ToArray()
            .Select(x => x == tileId ? -1 : x)
            .ToArray();
          packedTileData.SetTileMask(layer, TileMask.FromArray(updatedTileMaskArray));
        }
      }
    }
  }
}
