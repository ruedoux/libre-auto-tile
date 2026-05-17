using System.Collections.Frozen;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling;

/// <summary>
/// Class that manages auto tiling. Thread safe.
/// </summary>
public class AutoTiler
{
  private static readonly Vector2[] CELL_SURROUNDING_DIRECTIONS = [
    Vector2.TopLeft, Vector2.Top, Vector2.TopRight, Vector2.Right, Vector2.BottomRight, Vector2.Bottom, Vector2.BottomLeft, Vector2.Left];

  private readonly FrozenDictionary<int, TileSearcher> tileSearcherById;
  private readonly IAutoTilerData autoTilerData;

  private readonly object[] layerToLock;

  /// <summary>
  /// Leaving mapSize as default (0,0) makes the map dynamic (dictionary).
  /// Passing a size makes the map static size (array).
  /// </summary>
  public AutoTiler(
    int layerCount,
    IReadOnlyDictionary<int, TileSearcher> tileIdToTileMaskSearcher,
    Vector2 mapSize = default)
  {
    if (layerCount < 1)
      throw new ArgumentOutOfRangeException(nameof(layerCount), $"Layer count must be at least 1, given: {layerCount}");

    if (mapSize == default)
      autoTilerData = new AutoTilerDataDynamic(layerCount);
    else
      autoTilerData = new AutoTilerDataStatic(layerCount, mapSize);

    tileSearcherById = tileIdToTileMaskSearcher.ToFrozenDictionary();
    layerToLock = new object[layerCount];
    for (int i = 0; i < layerCount; i++)
      layerToLock[i] = new object();
  }

  public void Clear()
    => LockAllLayers(0, () => autoTilerData.Clear());

  public int GetLayerCount()
    => autoTilerData.LayerCount;

  public Vector2[] GetAllPositions(int layer)
  {
    ValidateLayer(layer);
    lock (layerToLock[layer])
    {
      return autoTilerData.GetAllPositions(layer);
    }
  }

  public TileData GetTile(int layer, Vector2 position)
  {
    ValidateLayer(layer);
    lock (layerToLock[layer])
    {
      return GetTileDataAt(layer, position);
    }
  }

  public Vector2[] PlaceTiles(int layer, IEnumerable<(Vector2 Position, int TileId)> tiles)
  {
    ValidateLayer(layer);
    var tileArray = tiles as (Vector2 Position, int TileId)[] ?? [.. tiles];
    foreach (var (_, tileId) in tileArray)
      ValidateTileId(tileId);

    lock (layerToLock[layer])
    {
      HashSet<Vector2> dirtyPositions = [];
      foreach (var (position, tileId) in tileArray)
      {
        if (tileId < 0)
          autoTilerData.Remove(layer, position);
        else
          autoTilerData.Set(layer, position, new(tileId, default, default));

        dirtyPositions.Add(position);
        for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
          dirtyPositions.Add(position + CELL_SURROUNDING_DIRECTIONS[i]);
      }

      foreach (var position in dirtyPositions)
        RecomputeTileAt(layer, position);

      return [.. dirtyPositions];
    }
  }

  private void RecomputeTileAt(int layer, Vector2 position)
  {
    TileData tileData = GetTileDataAt(layer, position);
    if (tileData.IsEmpty())
    {
      autoTilerData.Remove(layer, position);
      return;
    }

    Span<int> tileMaskArray = stackalloc int[8];
    for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
      tileMaskArray[i] = GetTileDataAt(layer, position + CELL_SURROUNDING_DIRECTIONS[i]).CentreTileId;

    TileMask tileMask = TileMask.FromArray(tileMaskArray);
    var bestMatch = tileSearcherById[tileData.CentreTileId].FindBestMatch(tileMask);
    autoTilerData.Set(layer, position, new(tileData.CentreTileId, tileMask, bestMatch.TileAtlas));
  }

  private TileData GetTileDataAt(int layer, Vector2 position)
    => autoTilerData.Get(layer, position);

  private void ValidateTileId(int tileId)
  {
    if (!tileSearcherById.ContainsKey(tileId) && tileId > -1)
      throw new ArgumentException($"Tile of id does not exist: {tileId}");
  }

  private void ValidateLayer(int layer)
  {
    if (layer >= autoTilerData.LayerCount || layer < 0)
      throw new ArgumentOutOfRangeException(nameof(layer), $"AutoTiler does not contain layer: {layer}");
  }

  private void LockAllLayers(int index, Action action)
  {
    if (index == layerToLock.Length)
    {
      action();
      return;
    }

    lock (layerToLock[index])
    {
      LockAllLayers(index + 1, action);
    }
  }
}
