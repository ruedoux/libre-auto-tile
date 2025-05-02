using System.Collections.Frozen;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile;

public readonly struct TileAtlas(Vector2 position, string imageFileName)
{
  public readonly Vector2 Position { get; init; } = position;
  public readonly string ImageFileName { get; init; } = imageFileName;

  public bool Equals(TileAtlas other)
    => Position.Equals(other.Position)
      && string.Equals(ImageFileName, other.ImageFileName, StringComparison.Ordinal);

  public override bool Equals(object? obj)
      => obj is TileAtlas other && Equals(other);

  public override int GetHashCode()
      => HashCode.Combine(Position, ImageFileName);

  public override string ToString() => $"({Position}, {ImageFileName})";

  public static bool operator ==(TileAtlas left, TileAtlas right)
    => left.Equals(right);

  public static bool operator !=(TileAtlas left, TileAtlas right)
    => !(left == right);
}


public class TileMaskSearcher
{
  public readonly FrozenDictionary<TileMask, TileAtlas> ExistingMasks;
  private readonly List<(TileMask TileMask, TileAtlas TileAtlas)> items;
  private readonly FrozenDictionary<int, List<int>>[] tileIdToItemIndex;

  private readonly int[] itemIndexToBestScore;
  private readonly int[] itemIndexToSeenGeneration;
  private uint currentGeneration;

  public TileMaskSearcher(List<(TileMask TileMask, TileAtlas TileAtlas)> rawItems)
  {
    var existingMasksTemp = new Dictionary<TileMask, TileAtlas>();
    var sanitizedItems = new List<(TileMask, TileAtlas)>();
    foreach (var rawItem in rawItems)
      if (existingMasksTemp.TryAdd(rawItem.TileMask, rawItem.TileAtlas))
        sanitizedItems.Add(rawItem);

    ExistingMasks = existingMasksTemp.ToFrozenDictionary();
    items = sanitizedItems;

    itemIndexToBestScore = new int[items.Count];
    itemIndexToSeenGeneration = new int[items.Count];
    currentGeneration = 1;

    Dictionary<int, List<int>>[] tileIdToItemIndexTemp = new Dictionary<int, List<int>>[8];
    for (int field = 0; field < 8; field++)
      tileIdToItemIndexTemp[field] = [];

    for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
    {
      var m = items[itemIndex].TileMask;
      AddIndex(tileIdToItemIndexTemp, 0, m.TopLeft, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 1, m.Top, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 2, m.TopRight, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 3, m.Right, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 4, m.BottomRight, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 5, m.Bottom, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 6, m.BottomLeft, itemIndex);
      AddIndex(tileIdToItemIndexTemp, 7, m.Left, itemIndex);
    }

    tileIdToItemIndex = tileIdToItemIndexTemp.Select(d => d.ToFrozenDictionary()).ToArray();
  }

  private static void AddIndex(
    Dictionary<int, List<int>>[] tileIdToItemIndexTemp, int field, int tileId, int itemIndex)
  {
    var dict = tileIdToItemIndexTemp[field];
    if (!dict.TryGetValue(tileId, out var list))
    {
      list = [];
      dict[tileId] = list;
    }
    list.Add(itemIndex);
  }

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    if (ExistingMasks.TryGetValue(target, out var atlas))
      return (target, atlas);

    int bestScore = 0, bestIdx = -1;
    currentGeneration++;
    if (currentGeneration == 0)
    {
      Array.Clear(itemIndexToSeenGeneration, 0, itemIndexToSeenGeneration.Length);
      currentGeneration = 1;
    }

    for (int field = 0; field < 8; field++)
    {
      int val = field switch
      {
        0 => target.TopLeft,
        1 => target.Top,
        2 => target.TopRight,
        3 => target.Right,
        4 => target.BottomRight,
        5 => target.Bottom,
        6 => target.BottomLeft,
        7 => target.Left,
        _ => throw new InvalidOperationException()
      };

      if (!tileIdToItemIndex[field].TryGetValue(val, out var list))
        continue;

      foreach (var itemIndex in list)
      {
        if (itemIndexToSeenGeneration[itemIndex] != currentGeneration)
        {
          itemIndexToSeenGeneration[itemIndex] = (int)currentGeneration;
          itemIndexToBestScore[itemIndex] = 0;
        }

        itemIndexToBestScore[itemIndex]++;
        if (itemIndexToBestScore[itemIndex] > bestScore)
        {
          bestScore = itemIndexToBestScore[itemIndex];
          bestIdx = itemIndex;
        }
      }
    }

    return bestIdx < 0 ? new(new(), new()) : items[bestIdx];
  }
}