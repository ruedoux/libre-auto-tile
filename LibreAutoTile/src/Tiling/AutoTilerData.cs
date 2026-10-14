using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibreAutoTile.Configuration.Models;

namespace LibreAutoTile.Tiling;

internal class AutoTilerData
{
  private struct Chunk(TileData[] tiles, int nonEmptyCount)
  {
    public TileData[] Tiles = tiles;
    public int NonEmptyCount = nonEmptyCount;
  }

  public int LayerCount => chunks.Length;

  private readonly int chunkSize;
  private readonly int shift;
  private readonly int mask;
  private readonly int poolCapacity;
  private readonly Dictionary<long, Chunk>[] chunks;
  private readonly Queue<TileData[]>[] pool;

  public AutoTilerData(int layerCount, int chunkSize, int chunkPoolCapacity)
  {
    if (layerCount < 1)
      throw new ArgumentOutOfRangeException(nameof(layerCount));

    if (chunkSize < 1 || (chunkSize & (chunkSize - 1)) != 0)
      throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be a power of two and at least 1.");

    if (chunkPoolCapacity < 0)
      throw new ArgumentOutOfRangeException(nameof(chunkPoolCapacity), "Chunk pool capacity cannot be negative.");

    this.chunkSize = chunkSize;
    poolCapacity = chunkPoolCapacity;

    int value = chunkSize;
    while (value > 1)
    {
      value >>= 1;
      shift++;
    }
    mask = chunkSize - 1;

    chunks = new Dictionary<long, Chunk>[layerCount];
    pool = new Queue<TileData[]>[layerCount];
    for (int i = 0; i < layerCount; i++)
    {
      chunks[i] = [];
      pool[i] = new();
    }
  }

  public TileData Get(int layer, Vector2 position)
  {
    if (!chunks[layer].TryGetValue(GetKey(position), out var chunk))
      return TileData.Empty;
    return chunk.Tiles[GetLocalIndex(position)];
  }

  public TileData GetCenterAndSurrounding(
    int layer,
    Vector2 position,
    ReadOnlySpan<Vector2> surroundingDirections,
    Span<int> surroundingCentreTileIds)
  {
    long key = GetKey(position);
    ref var chunk = ref CollectionsMarshal.GetValueRefOrNullRef(chunks[layer], key);
    if (Unsafe.IsNullRef(ref chunk))
    {
      for (int i = 0; i < surroundingDirections.Length; i++)
        surroundingCentreTileIds[i] = Get(layer, position + surroundingDirections[i]).CentreTileId;
      return TileData.Empty;
    }

    TileData center = chunk.Tiles[GetLocalIndex(position)];
    for (int i = 0; i < surroundingDirections.Length; i++)
    {
      Vector2 neighbour = position + surroundingDirections[i];
      if (GetKey(neighbour) == key)
        surroundingCentreTileIds[i] = chunk.Tiles[GetLocalIndex(neighbour)].CentreTileId;
      else
        surroundingCentreTileIds[i] = Get(layer, neighbour).CentreTileId;
    }
    return center;
  }

  public bool Set(int layer, Vector2 position, TileData tileData)
  {
    long key = GetKey(position);
    ref var chunk = ref CollectionsMarshal.GetValueRefOrNullRef(chunks[layer], key);
    if (Unsafe.IsNullRef(ref chunk))
    {
      if (tileData.IsEmpty())
        return true;

      var array = RentChunkArray(layer);
      array[GetLocalIndex(position)] = tileData;
      chunks[layer].Add(key, new Chunk(array, 1));
      return true;
    }

    int localIndex = GetLocalIndex(position);
    ref var cell = ref chunk.Tiles[localIndex];
    bool wasEmpty = cell.IsEmpty();
    bool isEmpty = tileData.IsEmpty();
    cell = tileData;

    if (wasEmpty && !isEmpty)
      chunk.NonEmptyCount++;
    else if (!wasEmpty && isEmpty && --chunk.NonEmptyCount == 0)
    {
      var array = chunk.Tiles;
      chunks[layer].Remove(key);
      RecycleChunkArray(layer, array);
    }

    return true;
  }

  public bool Remove(int layer, Vector2 position)
  {
    long key = GetKey(position);
    ref var chunk = ref CollectionsMarshal.GetValueRefOrNullRef(chunks[layer], key);
    if (Unsafe.IsNullRef(ref chunk))
      return false;

    int localIndex = GetLocalIndex(position);
    ref var cell = ref chunk.Tiles[localIndex];
    if (cell.IsEmpty())
      return false;

    cell = TileData.Empty;
    if (--chunk.NonEmptyCount == 0)
    {
      var array = chunk.Tiles;
      chunks[layer].Remove(key);
      RecycleChunkArray(layer, array);
    }

    return true;
  }

  public void Clear()
  {
    for (int layer = 0; layer < chunks.Length; layer++)
    {
      foreach (var chunk in chunks[layer].Values)
      {
        Array.Fill(chunk.Tiles, TileData.Empty);
        RecycleChunkArray(layer, chunk.Tiles);
      }
      chunks[layer].Clear();
    }
  }

  public Vector2[] GetAllPositions(int layer)
  {
    List<Vector2> positions = [];
    foreach (var (key, chunk) in chunks[layer])
    {
      int chunkX = (int)key;
      int chunkY = (int)(key >> 32);
      for (int i = 0; i < chunk.Tiles.Length; i++)
      {
        if (chunk.Tiles[i].IsEmpty())
          continue;

        int localX = i & mask;
        int localY = i >> shift;
        positions.Add(new Vector2((chunkX << shift) + localX, (chunkY << shift) + localY));
      }
    }
    return [.. positions];
  }

  private long GetKey(Vector2 position)
  {
    int chunkX = position.X >> shift;
    int chunkY = position.Y >> shift;
    return ((long)(uint)chunkY << 32) | (uint)chunkX;
  }

  private int GetLocalIndex(Vector2 position)
    => ((position.Y & mask) * chunkSize) + (position.X & mask);

  private TileData[] RentChunkArray(int layer)
  {
    if (pool[layer].Count > 0)
      return pool[layer].Dequeue();

    var array = new TileData[chunkSize * chunkSize];
    Array.Fill(array, TileData.Empty);
    return array;
  }

  private void RecycleChunkArray(int layer, TileData[] array)
  {
    if (poolCapacity == 0)
      return;

    if (pool[layer].Count >= poolCapacity)
      pool[layer].Dequeue();

    pool[layer].Enqueue(array);
  }
}
