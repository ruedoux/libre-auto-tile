using System.Collections.Concurrent;
using System.Collections.Frozen;
using Qwaitumin.AutoTile.Configuration;
using static Qwaitumin.AutoTile.TileMask;

namespace Qwaitumin.AutoTile;

public struct TileData(
  int centreTileId = -1, TileMask tileMask = default, TileAtlas tileAtlas = default)
{
  public int CentreTileId { internal set; get; } = centreTileId;
  public TileMask TileMask { internal set; get; } = tileMask;
  public TileAtlas TileAtlas { internal set; get; } = tileAtlas;
}

public class AutoTiler
{
  public static readonly Vector2[] CELL_SURROUNDING_DIRECTIONS = [
      Vector2.TopLeft, Vector2.Top, Vector2.TopRight, Vector2.Right, Vector2.BottomRight, Vector2.Bottom, Vector2.BottomLeft, Vector2.Left ];

  private readonly FrozenDictionary<int, TileMaskSearcher> tileIdToTileMaskSearcher;
  private readonly ConcurrentDictionary<Vector2, TileData>[] data;

  public AutoTiler(uint layerCount, Dictionary<int, TileMaskSearcher> tileIdToTileMaskSearcher)
  {
    if (layerCount < 1)
      throw new ArgumentException($"Layer count must be higher than 1, given: {layerCount}");

    data = new ConcurrentDictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < data.Length; layer++)
      data[layer] = new();
    this.tileIdToTileMaskSearcher = tileIdToTileMaskSearcher.ToFrozenDictionary();
  }

  public void Clear()
  {
    for (int i = 0; i < data.Length; i++)
      data[i].Clear();
  }

  public int GetLayerCount()
    => data.Length;

  public Vector2[] GetAllPositions(int layer)
  {
    ValidateLayer(layer);
    return [.. data[layer].Keys];
  }

  public TileData GetTile(int layer, Vector2 position)
  {
    ValidateLayer(layer);
    return GetTileDataAt(layer, position);
  }

  public void PlaceTile(int layer, Vector2 position, int tileId)
  {
    ValidateLayer(layer);
    ValidateTileId(tileId);

    if (tileId < 0)
      data[layer].TryRemove(position, out _);
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
      var tileAtlas = tileIdToTileMaskSearcher[tileId].FindBestMatch(tileMask).TileAtlas;
      data[tileId][position] = new(
        tileId, tileMask, tileAtlas);
    }

    for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
      UpdateTileRelative(layer, position, (SurroundingDirection)i);
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
    tileDataToUpdate.TileAtlas = tileIdToTileMaskSearcher[tileDataToUpdate.CentreTileId]
        .FindBestMatch(tileDataToUpdate.TileMask).TileAtlas;
  }

  private TileData GetTileDataAt(int layer, Vector2 position)
  {
    if (data[layer].TryGetValue(position, out TileData TileData))
      return TileData;
    return default;
  }

  private void ValidateTileId(int tileId)
  {
    if (tileId < 0 && tileIdToTileMaskSearcher.ContainsKey(tileId))
      throw new ArgumentException($"Tile of id does not exist: {tileId}");
  }

  private void ValidateLayer(int layer)
  {
    if (data.Length < layer - 1 || layer < 0)
      throw new ArgumentException($"AutoTiler does not contain layer: {layer}");
  }
}