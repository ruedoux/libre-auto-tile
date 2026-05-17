using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.Tiling;

internal interface IAutoTilerData
{
  TileData Get(int layer, Vector2 position);
  bool Set(int layer, Vector2 position, TileData tileData);
  bool Remove(int layer, Vector2 position);
  void Clear();
  Vector2[] GetAllPositions(int layer);
  int LayerCount { get; }
}

internal class AutoTilerDataStatic : IAutoTilerData
{
  public int LayerCount => data.GetLength(2);

  private readonly Vector2 mapSize;
  private readonly TileData[,,] data;
  private readonly Vector2[] positions;

  public AutoTilerDataStatic(int layerCount, Vector2 mapSize)
  {
    if (layerCount < 1)
      throw new ArgumentOutOfRangeException(nameof(layerCount));

    if (mapSize.X < 1)
      throw new ArgumentOutOfRangeException(nameof(mapSize), "Width must be >= 1.");
    if (mapSize.Y < 1)
      throw new ArgumentOutOfRangeException(nameof(mapSize), "Height must be >= 1.");

    this.mapSize = mapSize;
    data = new TileData[mapSize.X, mapSize.Y, layerCount];
    Clear();

    positions = new Vector2[mapSize.X * mapSize.Y];
    for (int i = 0, y = 0; y < mapSize.Y; y++)
      for (int x = 0; x < mapSize.X; x++)
        positions[i++] = new Vector2(x, y);
  }

  public TileData Get(int layer, Vector2 position)
  {
    if (!HasPosition(position))
      return TileData.Empty;
    return data[position.X, position.Y, layer];
  }

  public bool Set(int layer, Vector2 position, TileData tileData)
  {
    if (!HasPosition(position))
      return false;

    data[position.X, position.Y, layer] = tileData;
    return true;
  }

  public bool Remove(int layer, Vector2 position)
  {
    if (!HasPosition(position))
      return false;

    var exists = !data[position.X, position.Y, layer].IsEmpty();
    data[position.X, position.Y, layer] = TileData.Empty;
    return exists;
  }

  public void Clear()
  {
    for (int x = 0; x < mapSize.X; x++)
      for (int y = 0; y < mapSize.Y; y++)
        for (int layer = 0; layer < LayerCount; layer++)
          data[x, y, layer] = TileData.Empty;
  }

  public Vector2[] GetAllPositions(int layer)
    => [.. positions];

  private bool HasPosition(Vector2 position)
    => !(position.X < 0 || position.X >= mapSize.X || position.Y < 0 || position.Y >= mapSize.Y);
}

internal class AutoTilerDataDynamic : IAutoTilerData
{
  public int LayerCount => data.Length;
  private readonly Dictionary<Vector2, TileData>[] data;

  public AutoTilerDataDynamic(int layerCount)
  {
    if (layerCount < 1)
      throw new ArgumentOutOfRangeException(nameof(layerCount));

    data = new Dictionary<Vector2, TileData>[layerCount];
    for (int i = 0; i < data.Length; i++) data[i] = [];
  }

  public TileData Get(int layer, Vector2 position)
    => data[layer].TryGetValue(position, out var tileData) ? tileData : TileData.Empty;


  public bool Set(int layer, Vector2 position, TileData tileData)
  {
    data[layer][position] = tileData;
    return true;
  }


  public bool Remove(int layer, Vector2 position)
    => data[layer].Remove(position);

  public void Clear()
  {
    foreach (var layer in data) layer.Clear();
  }

  public Vector2[] GetAllPositions(int layer)
    => [.. data[layer].Keys];
}