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
  private readonly AutoTilerData autoTilerData;

  private readonly object[] layerToLock;
  private readonly HashSet<Vector2>[] layerToDirtyPositions;

  /// <summary>
  /// chunkSize must be a power of two and controls the size of each dynamically allocated chunk.
  /// chunkPoolCapacity controls how many freed chunk arrays are kept for reuse (0 disables pooling).
  /// Memory note: a chunk holds chunkSize² × ~64 B. At the default 128² that is ~1 MB/chunk,
  /// so a pool of 64 retains up to ~64 MB of chunk arrays in steady state.
  /// </summary>
  public AutoTiler(
    int layerCount,
    IReadOnlyDictionary<int, TileSearcher> tileIdToTileMaskSearcher,
    int chunkSize = 128,
    int chunkPoolCapacity = 64)
  {
    if (layerCount < 1)
      throw new ArgumentOutOfRangeException(nameof(layerCount), $"Layer count must be at least 1, given: {layerCount}");

    autoTilerData = new AutoTilerData(layerCount, chunkSize, chunkPoolCapacity);

    tileSearcherById = tileIdToTileMaskSearcher.ToFrozenDictionary();
    layerToLock = new object[layerCount];
    layerToDirtyPositions = new HashSet<Vector2>[layerCount];
    for (int i = 0; i < layerCount; i++)
    {
      layerToLock[i] = new object();
      layerToDirtyPositions[i] = new(Vector2FastComparer.INSTANCE);
    }
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

  /// <summary>
  /// Places tiles on the given layer and recomputes the affected positions.
  /// The returned collection is a reusable buffer scoped to this layer; it is only
  /// valid until the next PlaceTiles or Clear call on the same layer, so it must be
  /// consumed synchronously (as AutoTileDrawer does).
  /// </summary>
  public IReadOnlyCollection<Vector2> PlaceTiles(int layer, IEnumerable<(Vector2 Position, int TileId)> tiles)
  {
    ValidateLayer(layer);
    var tileArray = tiles as (Vector2 Position, int TileId)[] ?? [.. tiles];
    foreach (var (_, tileId) in tileArray)
      ValidateTileId(tileId);

    lock (layerToLock[layer])
    {
      HashSet<Vector2> dirtyPositions = layerToDirtyPositions[layer];
      dirtyPositions.Clear();
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

      return dirtyPositions;
    }
  }

  private void RecomputeTileAt(int layer, Vector2 position)
  {
    Span<int> tileMaskArray = stackalloc int[8];
    TileData tileData = autoTilerData.GetCenterAndSurrounding(layer, position, CELL_SURROUNDING_DIRECTIONS, tileMaskArray);
    if (tileData.IsEmpty())
    {
      autoTilerData.Remove(layer, position);
      return;
    }

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
