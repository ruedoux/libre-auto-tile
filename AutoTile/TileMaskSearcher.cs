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
  private readonly bool stripCorners;

  private readonly int[] itemIndexToBestScore;
  private readonly int[] itemIndexToSeenGeneration;
  private uint currentGeneration;

  public TileMaskSearcher(
    List<(TileMask TileMask, TileAtlas TileAtlas)> rawItems, bool stripCorners = true)
  {
    this.stripCorners = stripCorners;

    var existingMasksTemp = new Dictionary<TileMask, TileAtlas>();
    var sanitizedItems = new List<(TileMask, TileAtlas)>();
    foreach (var rawItem in rawItems)
    {
      if (existingMasksTemp.TryAdd(rawItem.TileMask, rawItem.TileAtlas))
        sanitizedItems.Add(rawItem);

      if (stripCorners)
      {
        var strippedTileMask = StripCorners(rawItem.TileMask);
        if (existingMasksTemp.TryAdd(strippedTileMask, rawItem.TileAtlas))
          sanitizedItems.Add(new(strippedTileMask, rawItem.TileAtlas));
      }
    }

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

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    var firstRoundResult = GetHits(target);

    var tileMask = firstRoundResult.TileMask;
    var hitsMask = firstRoundResult.HitsMask;

    // Get rid of fields that were never hit
    int h0 = hitsMask.TopLeft == 0 ? tileMask.TopLeft : -1;
    int h1 = hitsMask.Top == 0 ? tileMask.Top : -1;
    int h2 = hitsMask.TopRight == 0 ? tileMask.TopRight : -1;
    int h3 = hitsMask.Right == 0 ? tileMask.Right : -1;
    int h4 = hitsMask.BottomRight == 0 ? tileMask.BottomRight : -1;
    int h5 = hitsMask.Bottom == 0 ? tileMask.Bottom : -1;
    int h6 = hitsMask.BottomLeft == 0 ? tileMask.BottomLeft : -1;
    int h7 = hitsMask.Left == 0 ? tileMask.Left : -1;

    return FindBestMatchRound(new TileMask(h0, h1, h2, h3, h4, h5, h6, h7));
  }

  private (TileMask TileMask, TileMask HitsMask) GetHits(TileMask target)
  {
    if (ExistingMasks.TryGetValue(target, out var _))
      return (target, TileMask.GetZero());

    int bestIndex = GetBestIndex(target);

    var bestMask = bestIndex != -1 ? items[bestIndex].TileMask : new();
    var hitsMask = GetHitsMask(target, bestMask);

    return (bestMask, hitsMask);
  }

  private (TileMask TileMask, TileAtlas TileAtlas) FindBestMatchRound(TileMask target)
  {
    if (stripCorners)
      target = StripCorners(target);

    if (ExistingMasks.TryGetValue(target, out var atlas))
      return (target, atlas);

    int bestIndex = GetBestIndex(target);

    var bestMask = bestIndex != -1 ? items[bestIndex].TileMask : new();
    var bestAtlas = bestIndex != -1 ? items[bestIndex].TileAtlas : new();
    return (bestMask, bestAtlas);
  }

  private int GetBestIndex(TileMask target)
  {
    currentGeneration++;
    if (currentGeneration == 0)
    {
      Array.Clear(itemIndexToSeenGeneration, 0, itemIndexToSeenGeneration.Length);
      currentGeneration = 1;
    }

    int bestScore = 0, bestIdx = -1;
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

    return bestIdx;
  }

  private static TileMask GetHitsMask(TileMask target, TileMask bestMask)
  {
    int h0 = target.TopLeft == bestMask.TopLeft ? 0 : -1;
    int h1 = target.Top == bestMask.Top ? 0 : -1;
    int h2 = target.TopRight == bestMask.TopRight ? 0 : -1;
    int h3 = target.Right == bestMask.Right ? 0 : -1;
    int h4 = target.BottomRight == bestMask.BottomRight ? 0 : -1;
    int h5 = target.Bottom == bestMask.Bottom ? 0 : -1;
    int h6 = target.BottomLeft == bestMask.BottomLeft ? 0 : -1;
    int h7 = target.Left == bestMask.Left ? 0 : -1;

    return new(h0, h1, h2, h3, h4, h5, h6, h7);
  }

  private static TileMask StripCorners(TileMask target)
  {
    int topLeft = target.TopLeft;
    int topRight = target.TopRight;
    int bottomRight = target.BottomRight;
    int bottomLeft = target.BottomLeft;

    if (target.Top == -1 || target.Left == -1) topLeft = -1;
    if (target.Top == -1 || target.Right == -1) topRight = -1;
    if (target.Bottom == -1 || target.Left == -1) bottomLeft = -1;
    if (target.Bottom == -1 || target.Right == -1) bottomRight = -1;

    return new(
      topLeft,
      target.Top,
      topRight,
      target.Right,
      bottomRight,
      target.Bottom,
      bottomLeft,
      target.Left);
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
}