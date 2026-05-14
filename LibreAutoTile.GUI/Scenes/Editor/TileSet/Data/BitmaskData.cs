using Godot;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Data;



public class BitmaskData
{
  private readonly Dictionary<int, (TileMask TileMask, int CentreTileId, uint Propability)>
    layerToFullTileMask = [];

  public void AddTileMask(int layer, TileMask tileMask)
  {
    SetTileMask(layer, tileMask);
    RemoveLayerIfEmpty(layer);
  }

  public void AddBitmask(int layer, int tileId, Vector2I bitmaskPosition)
  {
    if (bitmaskPosition == TileSetMath.MIDDLE)
    {
      SetCentreTileId(layer, tileId);
      return;
    }

    var direction = TileSetMath.PositionToDirection(bitmaskPosition);
    var tileMask = GetTileMask(layer);
    tileMask = TileMask.ConstructModified(tileMask, direction, tileId);
    SetTileMask(layer, tileMask);
  }

  public void SetCentreTileId(int layer, int tileId)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      fullTileMask = new(new(), tileId, 1);
    layerToFullTileMask[layer] = new(fullTileMask.TileMask, tileId, fullTileMask.Propability);
  }

  public void SetTileMask(int layer, TileMask tileMask)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      fullTileMask = new(tileMask, -1, 1);
    layerToFullTileMask[layer] = new(tileMask, fullTileMask.CentreTileId, fullTileMask.Propability);
  }

  public void SetProbability(int layer, uint probability)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      return;
    layerToFullTileMask[layer] = new(
      fullTileMask.TileMask, fullTileMask.CentreTileId, probability);
  }

  public void RemoveBitmask(int layer, Vector2I bitmaskPosition)
  {
    if (bitmaskPosition == TileSetMath.MIDDLE)
    {
      SetCentreTileId(layer, -1);
      RemoveLayerIfEmpty(layer);
      return;
    }

    var direction = TileSetMath.PositionToDirection(bitmaskPosition);
    var tileMask = GetTileMask(layer);
    tileMask = TileMask.ConstructModified(tileMask, direction, -1);
    SetTileMask(layer, tileMask);
    RemoveLayerIfEmpty(layer);
  }

  private void RemoveLayerIfEmpty(int layer)
  {
    if (GetTileMask(layer).ToArray().All(x => x < 0) && GetCentreTileId(layer) < 0)
      layerToFullTileMask.Remove(layer);
  }

  public KeyValuePair<int, (TileMask TileMask, int CentreTileId, uint Propability)>[] GetAll()
    => [.. layerToFullTileMask];

  public int GetCentreTileId(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      return -1;
    return fullTileMask.CentreTileId;
  }

  public TileMask GetTileMask(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      return new();
    return fullTileMask.TileMask;
  }

  public uint GetProbability(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var fullTileMask))
      return 0;
    return fullTileMask.Propability;
  }

  public int[] GetLayers()
    => [.. layerToFullTileMask.Keys];

  public bool IsEmpty()
    => layerToFullTileMask.Count == 0;

  public void RemoveTileId(int tileId)
  {
    foreach (var (layer, fullTileMask) in layerToFullTileMask)
    {
      var centreTileId = GetCentreTileId(layer);
      if (centreTileId == tileId)
        SetCentreTileId(layer, -1);

      var tileMask = GetTileMask(layer);
      var updatedTileMaskArray = tileMask.ToArray()
        .Select(x => x == tileId ? -1 : x)
        .ToArray();
      SetTileMask(layer, TileMask.FromArray(updatedTileMaskArray));
    }
  }

  public void ChangeTileId(int newId, int oldId)
  {
    foreach (var (layer, fullTileMask) in layerToFullTileMask)
    {
      var centreTileId = GetCentreTileId(layer);
      if (centreTileId == oldId)
        SetCentreTileId(layer, newId);

      var tileMask = GetTileMask(layer);
      var updatedTileMaskArray = tileMask.ToArray()
        .Select(x => x == oldId ? newId : x)
        .ToArray();
      SetTileMask(layer, TileMask.FromArray(updatedTileMaskArray));
    }
  }
}