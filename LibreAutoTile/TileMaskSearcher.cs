using System.Collections.Frozen;
using System.Collections.Immutable;
using Microsoft.VisualBasic;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile;

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

class IndexSearcher(int itemCount)
{
  const int TOP_SCORE = 3;
  const int LOW_SCORE = 2;
  const int NO_SCORE = 1;
  public readonly int[] ResultIndexToItemIndex = new int[itemCount];

  private readonly int[] itemIndexToBestScore = new int[itemCount];
  private readonly int[] itemIndexToSeenGeneration = new int[itemCount];
  private readonly int[] tileScore = new int[8];
  private readonly object _lock = new();
  private int currentGeneration = 1;


  public (int ResultCount, int BestScore) GetResultCount(
    TileMask target, FrozenDictionary<int, List<int>>[] tileIdToItemIndexs)
  {
    lock (_lock)
    {
      IncrementGeneration();
      int resultIndex = -1;
      int bestScore = 0;
      for (int i = 0; i < tileScore.Length; i++)
        tileScore[i] = i % 2 == 0 ? LOW_SCORE : TOP_SCORE; // default score 2 for not corner ids

      tileScore[(int)TileMask.SurroundingDirection.TopLeft] = target.IsTopLeftConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.TopRight] = target.IsTopRightConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.BottomLeft] = target.IsBottomLeftConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.BottomRight] = target.IsBottomRightConnected() ? TOP_SCORE : NO_SCORE;

      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        int tileId = target.GetTileIdByIndex(fieldIndex);
        if (!tileIdToItemIndexs[fieldIndex].TryGetValue(tileId, out var itemIndexList))
          continue;

        foreach (var itemIndex in itemIndexList)
        {
          if (itemIndexToSeenGeneration[itemIndex] != currentGeneration)
          {
            itemIndexToSeenGeneration[itemIndex] = currentGeneration;
            itemIndexToBestScore[itemIndex] = 0;
          }

          itemIndexToBestScore[itemIndex] += tileScore[fieldIndex];

          if (itemIndexToBestScore[itemIndex] > bestScore)
          {
            bestScore = itemIndexToBestScore[itemIndex];
            resultIndex = 0;
            ResultIndexToItemIndex[resultIndex] = itemIndex;
          }
          else if (itemIndexToBestScore[itemIndex] == bestScore)
          {
            bestScore = itemIndexToBestScore[itemIndex];
            ResultIndexToItemIndex[++resultIndex] = itemIndex;
          }
        }
      }

      return (resultIndex, bestScore);
    }
  }

  private void IncrementGeneration()
  {
    currentGeneration++;
    if (currentGeneration < 0)
    {
      Array.Clear(itemIndexToSeenGeneration, 0, itemIndexToSeenGeneration.Length);
      currentGeneration = 1;
    }
  }
}

public class TileMaskSearcher
{
  public readonly FrozenDictionary<TileMask, TileAtlas> ExistingMasks;
  private readonly ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items;
  private readonly FrozenDictionary<int, List<int>>[] tileIdToItemIndex;
  private readonly IndexSearcher indexSearcher;

  public TileMaskSearcher(List<(TileMask TileMask, TileAtlas TileAtlas)> rawItems)
  {
    ExistingMasks = rawItems
      .GroupBy(item => item.TileMask)
      .Select(g => g.First())
      .ToDictionary(item => item.TileMask, item => item.TileAtlas)
      .ToFrozenDictionary();
    items = ExistingMasks
      .Select(kvp => (TileMask: kvp.Key, TileAtlas: kvp.Value)).ToImmutableArray();
    indexSearcher = new(items.Length);
    tileIdToItemIndex = GetAssignedIndexes(items).Select(d => d.ToFrozenDictionary()).ToArray();
  }

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    if (ExistingMasks.TryGetValue(target, out var tileAtlas))
      return (target, tileAtlas);

    (int resultCount, int _) = indexSearcher.GetResultCount(target, tileIdToItemIndex);
    if (resultCount == -1)
      return (new(), new());

    int rawBestIndex = indexSearcher.ResultIndexToItemIndex[resultCount];
    var tileMask = rawBestIndex != -1 ? items[rawBestIndex].TileMask : new();
    var hitsMask = GetHitsMask(target, tileMask);

    // Get rid of fields that were never hit
    int h0 = hitsMask.TopLeft == 0 ? tileMask.TopLeft : -1;
    int h1 = hitsMask.Top == 0 ? tileMask.Top : -1;
    int h2 = hitsMask.TopRight == 0 ? tileMask.TopRight : -1;
    int h3 = hitsMask.Right == 0 ? tileMask.Right : -1;
    int h4 = hitsMask.BottomRight == 0 ? tileMask.BottomRight : -1;
    int h5 = hitsMask.Bottom == 0 ? tileMask.Bottom : -1;
    int h6 = hitsMask.BottomLeft == 0 ? tileMask.BottomLeft : -1;
    int h7 = hitsMask.Left == 0 ? tileMask.Left : -1;

    TileMask trimmedTarget = new(h0, h1, h2, h3, h4, h5, h6, h7);
    trimmedTarget = TileMask.StripCorners(trimmedTarget);

    if (ExistingMasks.TryGetValue(trimmedTarget, out var atlas))
      return (trimmedTarget, atlas);

    (int bestResultCount, int _) = indexSearcher.GetResultCount(target, tileIdToItemIndex);
    int bestIndex = indexSearcher.ResultIndexToItemIndex[bestResultCount];
    var bestMask = bestIndex != -1 ? items[bestIndex].TileMask : new();
    var bestAtlas = bestIndex != -1 ? items[bestIndex].TileAtlas : new();

    return (bestMask, bestAtlas);
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

  private static Dictionary<int, List<int>>[] GetAssignedIndexes(
    ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    var tileIdToItemIndexTemp = new Dictionary<int, List<int>>[8];
    for (int field = 0; field < 8; field++)
      tileIdToItemIndexTemp[field] = [];

    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex].TileMask;
      var tileMaskArray = tileMask.ToArray();
      for (int maskIndex = 0; maskIndex < tileMaskArray.Length; maskIndex++)
      {
        var tileId = tileMaskArray[maskIndex];
        var dict = tileIdToItemIndexTemp[maskIndex];
        if (!dict.TryGetValue(tileId, out var list))
        {
          list = [];
          dict[tileId] = list;
        }
        list.Add(itemIndex);
      }
    }

    return tileIdToItemIndexTemp;
  }
}