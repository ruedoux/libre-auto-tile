using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;

public readonly struct FullTileMask(TileMask tileMask, int centreTileId)
{
  public readonly TileMask TileMask = tileMask;
  public readonly int CentreTileId = centreTileId;
}

public class GuiTileData
{
  public readonly Dictionary<int, FullTileMask> LayerFullTileMask = [];

  public void AddTileMask(int layer, TileMask tileMask)
  {
    var tileMaskArray = tileMask.ToArray();
    for (int i = 0; i < tileMaskArray.Length; i++)
    {
      var bitmaskPosition = BitmaskCalculator.DirectionToPosition(
        (TileMask.SurroundingDirection)i);
      AddBitmask(layer, tileMaskArray[i], bitmaskPosition);
    }
  }

  public int GetCentreTileId(int layer)
  {
    if (!LayerFullTileMask.TryGetValue(layer, out var fullTileMask))
      return -1;
    return fullTileMask.CentreTileId;
  }

  public TileMask GetTileMask(int layer)
  {
    if (!LayerFullTileMask.TryGetValue(layer, out var fullTileMask))
      return new();
    return fullTileMask.TileMask;
  }

  public void SetCentreTileId(int layer, int tileId)
  {
    if (!LayerFullTileMask.TryGetValue(layer, out var fullTileMask))
      fullTileMask = new(new(), tileId);
    LayerFullTileMask[layer] = new(fullTileMask.TileMask, tileId);
  }

  public void SetTileMask(int layer, TileMask tileMask)
  {
    if (!LayerFullTileMask.TryGetValue(layer, out var fullTileMask))
      fullTileMask = new(tileMask, -1);
    LayerFullTileMask[layer] = new(tileMask, fullTileMask.CentreTileId);
  }

  public void AddBitmask(int layer, int tileId, Vector2I bitmaskPosition)
  {
    if (bitmaskPosition == BitmaskCalculator.MIDDLE)
    {
      SetCentreTileId(layer, tileId);
      return;
    }

    var direction = BitmaskCalculator.PositionToDirection(bitmaskPosition);
    var tileMask = GetTileMask(layer);
    tileMask = TileMask.ConstructModified(tileMask, direction, tileId);
    SetTileMask(layer, tileMask);
  }

  public void RemoveBitmask(int layer, Vector2I bitmaskPosition)
  {
    if (bitmaskPosition == BitmaskCalculator.MIDDLE)
    {
      SetCentreTileId(layer, -1);
      RemoveLayerIfEmpty(layer);
      return;
    }

    var direction = BitmaskCalculator.PositionToDirection(bitmaskPosition);
    var tileMask = GetTileMask(layer);
    tileMask = TileMask.ConstructModified(tileMask, direction, -1);
    SetTileMask(layer, tileMask);
    RemoveLayerIfEmpty(layer);
  }

  private void RemoveLayerIfEmpty(int layer)
  {
    if (GetTileMask(layer).ToArray().All(x => x < 0) && GetCentreTileId(layer) < 0)
      LayerFullTileMask.Remove(layer);
  }
}