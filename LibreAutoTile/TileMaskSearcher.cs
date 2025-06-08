using System.Collections.Frozen;
using System.Collections.Immutable;
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


  public (int ResultCount, int BestScore) Search(
    TileMask target, FrozenDictionary<int, List<int>>[] tileIdToItemIndexes)
  {
    lock (_lock)
    {
      for (int i = 0; i < tileScore.Length; i++)
        tileScore[i] = i % 2 == 0 ? LOW_SCORE : TOP_SCORE; // default score 2 for not corner ids

      tileScore[(int)TileMask.SurroundingDirection.TopLeft] = target.IsTopLeftConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.TopRight] = target.IsTopRightConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.BottomLeft] = target.IsBottomLeftConnected() ? TOP_SCORE : NO_SCORE;
      tileScore[(int)TileMask.SurroundingDirection.BottomRight] = target.IsBottomRightConnected() ? TOP_SCORE : NO_SCORE;

      IncrementGeneration();
      int resultMaxIndex = -1;
      int bestScore = 0;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        // Get list of items that match template for current side id
        int tileId = target.GetTileIdByIndex(fieldIndex);
        if (!tileIdToItemIndexes[fieldIndex].TryGetValue(tileId, out var itemIndexList))
          continue;

        // Iterate over items that have tileId on a given field
        foreach (var itemIndex in itemIndexList)
        {
          // Reset score if in new generation
          if (itemIndexToSeenGeneration[itemIndex] != currentGeneration)
          {
            itemIndexToSeenGeneration[itemIndex] = currentGeneration;
            itemIndexToBestScore[itemIndex] = 0;
          }

          // Increase score for item
          itemIndexToBestScore[itemIndex] += tileScore[fieldIndex];
          var itemScore = itemIndexToBestScore[itemIndex];
          if (itemScore > bestScore)
          {
            bestScore = itemScore;
            resultMaxIndex = 0;
            ResultIndexToItemIndex[resultMaxIndex] = itemIndex;
          }
          else if (itemScore == bestScore)
          {
            bestScore = itemScore;
            ResultIndexToItemIndex[++resultMaxIndex] = itemIndex;
          }
        }
      }

      return (resultMaxIndex, bestScore);
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
  public const int WILD_CARD_ID = -2;

  public readonly FrozenDictionary<TileMask, TileAtlas> ExistingMasks;
  private readonly ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items;
  private readonly FrozenDictionary<int, List<int>>[] tileIdToItemIndex;
  private readonly IndexSearcher indexSearcher;

  public TileMaskSearcher(List<(TileMask TileMask, TileAtlas TileAtlas)> rawItems)
  {
    if (rawItems.Count == 0)
      throw new ArgumentException("TileMaskSearcher needs at least one item");

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

  /// <summary>
  /// If no field has a match returns first item (random tile)
  /// </summary>
  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    if (ExistingMasks.TryGetValue(target, out var tileAtlas))
      return (target, tileAtlas);

    // Could probably iterate over results that have same best score
    // and decide the best fit? For now pick last best score
    (int resultMaxIndex, int _) = indexSearcher.Search(target, tileIdToItemIndex);
    if (resultMaxIndex == -1)
      return items[0];

    int rawBestIndex = indexSearcher.ResultIndexToItemIndex[resultMaxIndex];
    var rawTileMask = rawBestIndex != -1 ? items[rawBestIndex].TileMask : new();

    // Get rid of fields that were never hit
    static int GetHitMask(int target, int rawTileMask)
      => (target == rawTileMask || rawTileMask == WILD_CARD_ID) ? rawTileMask : -1;

    int h0 = GetHitMask(target.TopLeft, rawTileMask.TopLeft);
    int h1 = GetHitMask(target.Top, rawTileMask.Top);
    int h2 = GetHitMask(target.TopRight, rawTileMask.TopRight);
    int h3 = GetHitMask(target.Right, rawTileMask.Right);
    int h4 = GetHitMask(target.BottomRight, rawTileMask.BottomRight);
    int h5 = GetHitMask(target.Bottom, rawTileMask.Bottom);
    int h6 = GetHitMask(target.BottomLeft, rawTileMask.BottomLeft);
    int h7 = GetHitMask(target.Left, rawTileMask.Left);

    TileMask trimmedTarget = new(h0, h1, h2, h3, h4, h5, h6, h7);
    trimmedTarget = TileMask.StripCorners(trimmedTarget);

    if (ExistingMasks.TryGetValue(trimmedTarget, out var atlas))
      return (trimmedTarget, atlas);

    (int trimmedResultMaxIndex, int _) = indexSearcher.Search(target, tileIdToItemIndex);
    int bestIndex = indexSearcher.ResultIndexToItemIndex[trimmedResultMaxIndex];

    return bestIndex != -1 ? items[bestIndex] : items[0];
  }

  private static Dictionary<int, List<int>>[] GetAssignedIndexes(
    ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    var tileIdToItemIndexTemp = new Dictionary<int, List<int>>[8];
    for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      tileIdToItemIndexTemp[fieldIndex] = [];

    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex].TileMask;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var tileId = tileMask.GetTileIdByIndex(fieldIndex);
        var dict = tileIdToItemIndexTemp[fieldIndex];
        if (!dict.TryGetValue(tileId, out var list))
        {
          list = [];
          dict[tileId] = list;
        }
        list.Add(itemIndex);
      }
    }

    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex].TileMask;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var tileId = tileMask.GetTileIdByIndex(fieldIndex);
        var dict = tileIdToItemIndexTemp[fieldIndex];
        if (tileId == WILD_CARD_ID)
          foreach (var keyTileId in dict.Keys)
            dict[keyTileId].Add(itemIndex);
      }
    }

    return tileIdToItemIndexTemp;
  }
}