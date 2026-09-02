using Godot;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public class BitmaskData
{
  private sealed record LayerData(TileMask TileMask, int CentreTileId, uint Probability);

  private readonly Dictionary<int, LayerData> layerToFullTileMask = [];

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
    var tileMask = TileMask.ConstructModified(GetTileMask(layer), direction, tileId);
    SetTileMask(layer, tileMask);
  }

  public void SetCentreTileId(int layer, int tileId)
    => layerToFullTileMask[layer] = GetOrCreate(layer) with { CentreTileId = tileId };

  public void SetTileMask(int layer, TileMask tileMask)
    => layerToFullTileMask[layer] = GetOrCreate(layer) with { TileMask = tileMask };

  public void SetProbability(int layer, uint probability)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var data))
      return;
    layerToFullTileMask[layer] = data with { Probability = probability };
  }

  private LayerData GetOrCreate(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var data))
    {
      data = new(new(), -1, 1);
      layerToFullTileMask[layer] = data;
    }
    return data;
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
    var tileMask = TileMask.ConstructModified(GetTileMask(layer), direction, -1);
    SetTileMask(layer, tileMask);
    RemoveLayerIfEmpty(layer);
  }

  private void RemoveLayerIfEmpty(int layer)
  {
    if (GetTileMask(layer).ToArray().All(x => x < 0) && GetCentreTileId(layer) < 0)
      layerToFullTileMask.Remove(layer);
  }

  public KeyValuePair<int, (TileMask TileMask, int CentreTileId, uint Probability)>[] GetAll()
    => [.. layerToFullTileMask.Select(kvp =>
      new KeyValuePair<int, (TileMask, int, uint)>(
        kvp.Key, (kvp.Value.TileMask, kvp.Value.CentreTileId, kvp.Value.Probability)))];

  public int GetCentreTileId(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var data))
      return -1;
    return data.CentreTileId;
  }

  public TileMask GetTileMask(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var data))
      return new();
    return data.TileMask;
  }

  public uint GetProbability(int layer)
  {
    if (!layerToFullTileMask.TryGetValue(layer, out var data))
      return 0;
    return data.Probability;
  }

  public int[] GetLayers()
    => [.. layerToFullTileMask.Keys];

  public bool IsEmpty()
    => layerToFullTileMask.Count == 0;

  public void RemoveTileId(int tileId)
  {
    foreach (var (layer, _) in layerToFullTileMask)
    {
      if (GetCentreTileId(layer) == tileId)
        SetCentreTileId(layer, -1);

      var tileMaskArray = GetTileMask(layer).ToArray()
        .Select(x => x == tileId ? -1 : x)
        .ToArray();
      SetTileMask(layer, TileMask.FromArray(tileMaskArray));
    }
  }

  public void ChangeTileId(int newId, int oldId)
  {
    foreach (var (layer, _) in layerToFullTileMask)
    {
      if (GetCentreTileId(layer) == oldId)
        SetCentreTileId(layer, newId);

      var tileMaskArray = GetTileMask(layer).ToArray()
        .Select(x => x == oldId ? newId : x)
        .ToArray();
      SetTileMask(layer, TileMask.FromArray(tileMaskArray));
    }
  }
}
