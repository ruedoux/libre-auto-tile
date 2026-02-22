using System.Collections.Frozen;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling.Search;

namespace Qwaitumin.LibreAutoTile.Tiling;

/// <summary>
/// Class that manages auto tiling. Thread safe.
/// </summary>
public class AutoTiler
{
  public static readonly Vector2[] CELL_SURROUNDING_DIRECTIONS = [
      Vector2.TopLeft, Vector2.Top, Vector2.TopRight, Vector2.Right, Vector2.BottomRight, Vector2.Bottom, Vector2.BottomLeft, Vector2.Left ];

  private readonly FrozenDictionary<int, TileSearcher> tileIdToTileMaskSearcher;
  private readonly Dictionary<Vector2, TileData>[] data; // TODO, maybe give option to make 2D array?
  private readonly ReaderWriterLockSlim readWriteLock = new();
  private readonly int[] tileMaskArray; // Assuming this is accessed only with lock!


  public AutoTiler(uint layerCount, AutoTileConfiguration autoTileConfiguration)
    : this(layerCount, BuildTileIdToTileMaskSearcher(autoTileConfiguration)) { }

  public AutoTiler(uint layerCount, Dictionary<int, TileSearcher> tileIdToTileMaskSearcher)
  {
    if (layerCount < 1)
      throw new ArgumentException($"Layer count must be higher than 1, given: {layerCount}");

    data = new Dictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < data.Length; layer++)
      data[layer] = [];

    this.tileIdToTileMaskSearcher = tileIdToTileMaskSearcher.ToFrozenDictionary();
    tileMaskArray = new TileMask().ToArray();
  }

  private static Dictionary<int, TileSearcher> BuildTileIdToTileMaskSearcher(
    AutoTileConfiguration autoTileConfiguration)
  {
    var connectionGroupToTileIds = autoTileConfiguration.TileDefinitions
      .Where(td => td.Value.ConnectionGroup != null)
      .GroupBy(td => td.Value.ConnectionGroup!.Value)
      .ToDictionary(
          g => g.Key,
          g => new HashSet<int>(g.Select(td => (int)td.Key))
      );

    Dictionary<int, TileSearcher> tileIdToTileSearcher = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      List<(TileMask TileMask, TileAtlas TileAtlas)> items = [];
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        foreach (var (position, tileMaskArrays) in tileMaskDefinition.AtlasPositionToTileMasks)
        {
          foreach (var tileMaskArray in tileMaskArrays)
          {
            TileMask tileMask = TileMask.FromArray([.. tileMaskArray]);
            int tileAtlasChance = tileMaskDefinition.AtlasPositionToChance.TryGetValue(position, out var chance) ? chance : int.MaxValue;
            TileAtlas tileAtlas = new(position.ToVector2(), imageFileName, tileAtlasChance);
            items.Add(new(tileMask, tileAtlas));
          }
        }
      }

      HashSet<int>? connectionGroupArray = null;
      if (tileDefinition.ConnectionGroup is not null)
        connectionGroupArray = connectionGroupToTileIds[(uint)tileDefinition.ConnectionGroup];

      TileMaskSearcher tileMaskSearcher = new(
        items.Select(x => x.TileMask), connectionGroupArray, autoTileConfiguration.WildcardId);
      TileAtlasResolver tileAtlasResolver = new(items);
      tileIdToTileSearcher.Add((int)tileId, new(tileMaskSearcher, tileAtlasResolver));
    }
    return tileIdToTileSearcher;
  }

  public void Clear()
  {
    readWriteLock.EnterWriteLock();
    try
    {
      for (int i = 0; i < data.Length; i++)
        data[i].Clear();
    }
    finally
    {
      readWriteLock.ExitWriteLock();
    }
  }

  public int GetLayerCount()
    => data.Length;

  public Vector2[] GetAllPositions(int layer)
  {
    ValidateLayer(layer);
    readWriteLock.EnterReadLock();
    try
    {
      return [.. data[layer].Keys];
    }
    finally
    {
      readWriteLock.ExitReadLock();
    }
  }

  public TileData GetTile(int layer, Vector2 position)
  {
    ValidateLayer(layer);
    readWriteLock.EnterReadLock();
    try
    {
      return GetTileDataAt(layer, position);
    }
    finally
    {
      readWriteLock.ExitReadLock();
    }
  }

  public void PlaceTile(int layer, Vector2 position, int tileId)
  {
    ValidateLayer(layer);
    ValidateTileId(tileId);

    readWriteLock.EnterWriteLock();
    try
    {
      if (tileId < 0)
        data[layer].Remove(position, out _);
      else
      {
        for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
        {
          var surroundingTileData = GetTileDataAt(layer, position + CELL_SURROUNDING_DIRECTIONS[i]);
          var surroundingTileId = surroundingTileData.CentreTileId;
          tileMaskArray[i] = surroundingTileId;
        }

        TileMask tileMask = TileMask.FromArray(tileMaskArray);
        var bestMatch = tileIdToTileMaskSearcher[tileId].FindBestMatch(tileMask);
        data[layer][position] = new(
          tileId, tileMask, bestMatch.TileAtlas);
      }

      for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
        UpdateTileRelative(layer, position, (TileMask.SurroundingDirection)i);
    }
    finally
    {
      readWriteLock.ExitWriteLock();
    }
  }


  private void UpdateTileRelative(
    int layer, Vector2 centerPosition, TileMask.SurroundingDirection updateDirection)
  {
    Vector2 updatePosition = centerPosition - CELL_SURROUNDING_DIRECTIONS[(int)updateDirection];
    TileData tileDataToUpdate = GetTileDataAt(layer, updatePosition);
    TileData centerTileData = GetTileDataAt(layer, centerPosition);
    TileMask updatedTileMask = TileMask.ConstructModified(
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