using System.Collections.Concurrent;
using System.Numerics;


namespace Qwaitumin.AutoTile;

public sealed record TileData(int TileId, byte Bitmask, Vector2 AtlasCoords);

public static class Vector2Directions
{
  public static Vector2 Zero => new(0, 0);
  public static Vector2 One => new(1, 1);
  public static Vector2 TopLeft => new(-1, -1);
  public static Vector2 Top => new(0, -1);
  public static Vector2 TopRight => new(1, -1);
  public static Vector2 Right => new(1, 0);
  public static Vector2 BottomRight => new(1, 1);
  public static Vector2 Bottom => new(0, 1);
  public static Vector2 BottomLeft => new(-1, 1);
  public static Vector2 Left => new(-1, 0);
}

public class AutoTiler
{
  public static readonly Vector2[] CELL_SURROUNDING_DIRECTIONS = new Vector2[] {
      Vector2Directions.TopLeft, Vector2Directions.Top, Vector2Directions.TopRight, Vector2Directions.Right, Vector2Directions.BottomRight, Vector2Directions.Bottom, Vector2Directions.BottomLeft, Vector2Directions.Left };

  private readonly AutoTileData[] tileIdToAutoTileData;
  private readonly ConcurrentDictionary<Vector2, TileData>[] data;

  public AutoTiler(uint layerCount, AutoTileData[] tileIdToAutoTileData)
  {
    if (layerCount < 1)
      throw new ArgumentException($"Layer count must be higher than 1, given: {layerCount}");

    data = new ConcurrentDictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < data.Length; layer++)
      data[layer] = new();

    this.tileIdToAutoTileData = tileIdToAutoTileData;
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
    return data[layer].Keys.ToArray();
  }

  public TileData? GetTile(int layer, Vector2 position)
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
      data[layer][position] = new(tileId, new(), Vector2.Zero);

      var bitmask = GetTileBitmask(layer, position);
      data[layer][position] = new(
        tileId, bitmask, tileIdToAutoTileData[tileId].GetAtlasCoords(Bitmask.Parse(bitmask)));
    }

    for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
      UpdateTileBitmaskRelative(layer, position, (Bitmask.SurroundingDirection)i);
  }

  private void UpdateTileBitmaskRelative(
    int layer, Vector2 centerPosition, Bitmask.SurroundingDirection updateDirection)
  {
    Vector2 updatePosition = centerPosition - CELL_SURROUNDING_DIRECTIONS[(int)updateDirection];
    if (GetTileDataAt(layer, updatePosition) is not TileData tileToUpdate)
      return;

    TileData? centerTile = GetTileDataAt(layer, centerPosition);
    bool canConnect = centerTile is not null && tileIdToAutoTileData[tileToUpdate.TileId].CanConnectTo(centerTile.TileId);
    byte bitmask = Bitmask.UpdateBitmask(tileToUpdate.Bitmask, updateDirection, canConnect);
    data[layer][updatePosition] = new(
      tileToUpdate.TileId,
      bitmask,
      tileIdToAutoTileData[tileToUpdate.TileId].GetAtlasCoords(Bitmask.Parse(bitmask)));
  }

  private byte GetTileBitmask(int layer, Vector2 position)
  {
    TileData? centerTile = GetTileDataAt(layer, position);
    if (centerTile is null)
      return Bitmask.DEFAULT;

    var bitmaskArr = new bool[8];
    for (int i = 0; i < CELL_SURROUNDING_DIRECTIONS.Length; i++)
    {
      var surroundingPosition = position + CELL_SURROUNDING_DIRECTIONS[i];
      TileData? tileData = GetTileDataAt(layer, surroundingPosition);
      if (tileData is null)
        continue;

      bitmaskArr[i] = tileIdToAutoTileData[centerTile.TileId].CanConnectTo(tileData.TileId);
    }

    return Bitmask.FromArray(bitmaskArr);
  }

  private TileData? GetTileDataAt(int layer, Vector2 position)
  {
    if (data[layer].TryGetValue(position, out TileData? tileData))
      return tileData;
    return null;
  }

  private void ValidateTileId(int tileId)
  {
    if (tileIdToAutoTileData.Length <= tileId)
      throw new ArgumentException($"Tile of id does not exist: {tileId}, max tile id is: {tileIdToAutoTileData.Length - 1}");
  }

  private void ValidateLayer(int layer)
  {
    if (data.Length < layer - 1 || layer < 0)
      throw new ArgumentException($"AutoTiler does not contain layer: {layer}");
  }
}