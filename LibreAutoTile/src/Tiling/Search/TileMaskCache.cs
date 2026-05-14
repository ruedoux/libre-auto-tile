using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

public sealed class TileMaskCache
{
  private struct Slot
  {
    public TileMask Key;
    public TileMask Value;
  }

  private readonly Dictionary<TileMask, int> map;
  private readonly Slot[] slots;
  private readonly FixedLruList lru;
  private readonly object sync = new();

  public TileMaskCache(int capacity)
  {
    if (capacity <= 0)
      throw new ArgumentOutOfRangeException(nameof(capacity));

    map = new Dictionary<TileMask, int>(capacity);
    map.EnsureCapacity(capacity);

    slots = new Slot[capacity];
    lru = new FixedLruList(capacity);
  }

  public bool TryGet(TileMask key, out TileMask value)
  {
    lock (sync)
    {
      if (!map.TryGetValue(key, out int index))
      {
        value = default;
        return false;
      }

      lru.MoveToHead(index);
      value = slots[index].Value;
      return true;
    }
  }

  public void Set(TileMask key, TileMask value)
  {
    lock (sync)
    {
      if (map.TryGetValue(key, out int index))
      {
        slots[index].Value = value;
        lru.MoveToHead(index);
        return;
      }

      if (lru.TryTakeFree(out index))
      {
        slots[index].Key = key;
        slots[index].Value = value;
        lru.AddToHead(index);
        map.Add(key, index);
        return;
      }

      index = lru.Tail;

      map.Remove(slots[index].Key);

      slots[index].Key = key;
      slots[index].Value = value;

      lru.MoveToHead(index);
      map.Add(key, index);
    }
  }
}

internal sealed class FixedLruList
{
  private struct Node
  {
    public int Prev;
    public int Next;
    public bool InUse;
  }

  private readonly Node[] nodes;
  private int head;
  private int tail;
  private int freeHead;

  public int Tail
  {
    get
    {
      if (tail < 0)
        throw new InvalidOperationException("LRU list is empty.");
      return tail;
    }
  }

  public FixedLruList(int capacity)
  {
    if (capacity <= 0)
      throw new ArgumentOutOfRangeException(nameof(capacity));

    nodes = new Node[capacity];
    head = -1;
    tail = -1;
    freeHead = 0;

    for (int i = 0; i < capacity; i++)
    {
      nodes[i].Prev = -1;
      nodes[i].Next = i + 1 < capacity ? i + 1 : -1;
      nodes[i].InUse = false;
    }
  }

  public bool TryTakeFree(out int index)
  {
    if (freeHead < 0)
    {
      index = -1;
      return false;
    }

    index = freeHead;
    freeHead = nodes[index].Next;

    nodes[index].Prev = -1;
    nodes[index].Next = -1;
    nodes[index].InUse = false; // still not active until AddToHead
    return true;
  }

  public void AddToHead(int index)
  {
    if ((uint)index >= (uint)nodes.Length)
      throw new ArgumentOutOfRangeException(nameof(index));

    if (nodes[index].InUse)
      throw new InvalidOperationException("Node is already in the active LRU list.");

    nodes[index].Prev = -1;
    nodes[index].Next = head;
    nodes[index].InUse = true;

    if (head >= 0)
      nodes[head].Prev = index;
    else
      tail = index;

    head = index;
  }

  public void MoveToHead(int index)
  {
    if ((uint)index >= (uint)nodes.Length)
      throw new ArgumentOutOfRangeException(nameof(index));

    if (!nodes[index].InUse)
      throw new InvalidOperationException("Node is not in the active LRU list.");

    if (head == index)
      return;

    int prev = nodes[index].Prev;
    int next = nodes[index].Next;

    if (prev >= 0)
      nodes[prev].Next = next;

    if (next >= 0)
      nodes[next].Prev = prev;
    else
      tail = prev;

    nodes[index].Prev = -1;
    nodes[index].Next = head;

    if (head >= 0)
      nodes[head].Prev = index;

    head = index;
  }
}