using System.Collections.Concurrent;
using System.Collections.Frozen;
using Qwaitumin.LibreAutoTile.Configuration;
using static Qwaitumin.LibreAutoTile.TileMask;

namespace Qwaitumin.LibreAutoTile;

public struct TileData
{
  public int CentreTileId { get; set; } = -1;
  public TileMask TileMask { get; set; } = new();
  public TileAtlas TileAtlas { get; set; } = new();

  public TileData() { }

  public TileData(int centreTileId, TileMask tileMask, TileAtlas tileAtlas) : this()
  {
    CentreTileId = centreTileId;
    TileMask = tileMask;
    TileAtlas = tileAtlas;
  }

  public override readonly string ToString()
    => $"\"CentreTileId\":{CentreTileId}, \"TileMask\":{TileMask}, \"TileAtlas\":{TileAtlas}";
}

/// <summary>
/// Class that manages auto tiling. Thread safe.
/// </summary>
public class AutoTiler
{
  public static readonly Vector2[] CELL_SURROUNDING_DIRECTIONS = [
      Vector2.TopLeft, Vector2.Top, Vector2.TopRight, Vector2.Right, Vector2.BottomRight, Vector2.Bottom, Vector2.BottomLeft, Vector2.Left ];

  private readonly FrozenDictionary<int, TileMaskSearcher> tileIdToTileMaskSearcher;
  private readonly Dictionary<Vector2, TileData>[] data;
  private readonly ReaderWriterLockSlim _lock = new();

  public AutoTiler(uint layerCount, Dictionary<int, TileMaskSearcher> tileIdToTileMaskSearcher)
  {
    if (layerCount < 1)
      throw new ArgumentException($"Layer count must be higher than 1, given: {layerCount}");

    data = new Dictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < data.Length; layer++)
      data[layer] = [];
    this.tileIdToTileMaskSearcher = tileIdToTileMaskSearcher.ToFrozenDictionary();
  }

  public void Clear()
  {
    _lock.EnterWriteLock();
    try
    {
      for (int i = 0; i < data.Length; i++)
        data[i].Clear();
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }

  public int GetLayerCount()
    => data.Length;

  public Vector2[] GetAllPositions(int layer)
  {
    ValidateLayer(layer);
    _lock.EnterReadLock();
    try
    {
      return [.. data[layer].Keys];
    }
    finally
    {
      _lock.ExitReadLock();
    }
  }

  public TileData GetTile(int layer, Vector2 position)
  {
    ValidateLayer(layer);
    _lock.EnterReadLock();
    try
    {
      return GetTileDataAt(layer, position);
    }
    finally
    {
      _lock.ExitReadLock();
    }
  }

  public void PlaceTile(int layer, Vector2 position, int tileId)
  {
    ValidateLayer(layer);
    ValidateTileId(tileId);

    _lock.EnterWriteLock();
    try
    {
      if (tileId < 0)
        data[layer].Remove(position, out _);
      else
      {
        int[] tileMaskArray = new int[8];
        for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
        {
          var surroundingTileData = GetTileDataAt(layer, position + CELL_SURROUNDING_DIRECTIONS[i]);
          var surroundingTileId = surroundingTileData.CentreTileId;
          tileMaskArray[i] = surroundingTileId;
        }

        TileMask tileMask = FromArray(tileMaskArray);
        var bestMatch = tileIdToTileMaskSearcher[tileId].FindBestMatch(tileMask);
        data[layer][position] = new(
          tileId, tileMask, bestMatch.TileAtlas);
      }

      for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
        UpdateTileRelative(layer, position, (SurroundingDirection)i);
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }


  private void UpdateTileRelative(
    int layer, Vector2 centerPosition, SurroundingDirection updateDirection)
  {
    Vector2 updatePosition = centerPosition - CELL_SURROUNDING_DIRECTIONS[(int)updateDirection];
    TileData tileDataToUpdate = GetTileDataAt(layer, updatePosition);
    TileData centerTileData = GetTileDataAt(layer, centerPosition);
    TileMask updatedTileMask = ConstructModified(
      tileDataToUpdate.TileMask,
      updateDirection,
      centerTileData.CentreTileId);

    tileDataToUpdate.TileMask = updatedTileMask;
    if (tileIdToTileMaskSearcher.TryGetValue(tileDataToUpdate.CentreTileId, out var tileMaskSearcher))
      tileDataToUpdate.TileAtlas = tileMaskSearcher.FindBestMatch(updatedTileMask).TileAtlas;
    else
      tileDataToUpdate.TileAtlas = new();
    data[layer][updatePosition] = tileDataToUpdate;
  }

  private TileData GetTileDataAt(int layer, Vector2 position)
  {
    if (data[layer].TryGetValue(position, out var tileData))
      return tileData;
    return new();
  }

  private void ValidateTileId(int tileId)
  {
    if (!tileIdToTileMaskSearcher.ContainsKey(tileId) && tileId > -1)
      throw new ArgumentException($"Tile of id does not exist: {tileId}");
  }

  private void ValidateLayer(int layer)
  {
    if (data.Length < layer - 1 || layer < 0)
      throw new ArgumentException($"AutoTiler does not contain layer: {layer}");
  }
}