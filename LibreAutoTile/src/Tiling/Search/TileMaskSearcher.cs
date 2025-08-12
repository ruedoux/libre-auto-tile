using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;


/// <summary>
/// Finds best fitting atlas for a provided mask. Thread safe.
/// </summary>
public class TileMaskSearcher
{
  public const int DEFAULT_WILDCARD_ID = -2;

  public readonly FrozenDictionary<TileMask, TileAtlas> ExistingMasks;
  private readonly ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items;
  private readonly IndexSearcher indexSearcher;
  private readonly FrozenSet<int> connectionGroupTileIds;

  public TileMaskSearcher(
    IEnumerable<(TileMask TileMask, TileAtlas TileAtlas)> rawItems,
    HashSet<int> connectionGroupTileIds,
    uint? wildcardId = null)
  {
    int parsedWildcardId = (int?)wildcardId ?? DEFAULT_WILDCARD_ID;
    ExistingMasks = rawItems
      .GroupBy(item => item.TileMask)
      .Select(g => g.First())
      .ToDictionary(item => item.TileMask, item => item.TileAtlas)
      .ToFrozenDictionary();
    items = ExistingMasks
      .Select(kvp => (TileMask: kvp.Key, TileAtlas: kvp.Value)).ToImmutableArray();
    indexSearcher = new(
      items.Length,
      GetAssignedIndexes(items, parsedWildcardId).Select(d => d.ToFrozenDictionary()).ToArray(),
      parsedWildcardId);
    this.connectionGroupTileIds = connectionGroupTileIds.ToFrozenSet();
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
    (int resultMaxIndex, int _) = indexSearcher.Search(target);
    TileMask parsedTarget = new();
    if (resultMaxIndex != -1)
    {
      int rawBestIndex = indexSearcher.ResultIndexToItemIndex[resultMaxIndex];
      var rawTileMask = rawBestIndex != -1 ? items[rawBestIndex].TileMask : new();
      parsedTarget = ParseTarget(target, rawTileMask, indexSearcher.WildcardId);
    }

    if (ExistingMasks.TryGetValue(parsedTarget, out var atlas))
      return (parsedTarget, atlas);

    (int trimmedResultMaxIndex, int _) = indexSearcher.Search(target);
    if (trimmedResultMaxIndex == -1)
      return GetDefaultItem();

    int bestIndex = indexSearcher.ResultIndexToItemIndex[trimmedResultMaxIndex];
    return bestIndex != -1 ? items[bestIndex] : GetDefaultItem();
  }

  private static TileMask ParseTarget(TileMask target, TileMask rawTileMask, int wildcardId)
  {
    int GetHitMask(int target, int rawTileMask)
      => (target == rawTileMask || rawTileMask == wildcardId) ? rawTileMask : -1;

    int h0 = GetHitMask(target.TopLeft, rawTileMask.TopLeft);
    int h1 = GetHitMask(target.Top, rawTileMask.Top);
    int h2 = GetHitMask(target.TopRight, rawTileMask.TopRight);
    int h3 = GetHitMask(target.Right, rawTileMask.Right);
    int h4 = GetHitMask(target.BottomRight, rawTileMask.BottomRight);
    int h5 = GetHitMask(target.Bottom, rawTileMask.Bottom);
    int h6 = GetHitMask(target.BottomLeft, rawTileMask.BottomLeft);
    int h7 = GetHitMask(target.Left, rawTileMask.Left);

    TileMask parsedTarget = new(h0, h1, h2, h3, h4, h5, h6, h7);
    return TileMask.StripCorners(parsedTarget);
  }

  private static Dictionary<int, List<int>>[] GetAssignedIndexes(
    ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items, int wildcardId)
  {
    var tileIdToItemIndexesTemp = new Dictionary<int, List<int>>[8];
    for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      tileIdToItemIndexesTemp[fieldIndex] = [];

    AssignIndexes(items, tileIdToItemIndexesTemp);
    AssignWildcards(items, tileIdToItemIndexesTemp, wildcardId);

    return tileIdToItemIndexesTemp;
  }

  private static void AssignIndexes(
    ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items,
    Dictionary<int, List<int>>[] tileIdToItemIndexesTemp)
  {
    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex].TileMask;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var tileId = tileMask.GetTileIdByIndex(fieldIndex);
        var dict = tileIdToItemIndexesTemp[fieldIndex];
        if (!dict.TryGetValue(tileId, out var list))
        {
          list = [];
          dict[tileId] = list;
        }
        list.Add(itemIndex);
      }
    }
  }

  private static void AssignWildcards(
    ImmutableArray<(TileMask TileMask, TileAtlas TileAtlas)> items,
    Dictionary<int, List<int>>[] tileIdToItemIndexesTemp,
    int wildcardId)
  {
    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex].TileMask;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var tileId = tileMask.GetTileIdByIndex(fieldIndex);
        var dict = tileIdToItemIndexesTemp[fieldIndex];
        if (tileId == wildcardId)
          foreach (var keyTileId in dict.Keys)
            dict[keyTileId].Add(itemIndex);
      }
    }
  }

  private (TileMask TileMask, TileAtlas TileAtlas) GetDefaultItem()
    => items.Length > 0 ? items[0] : new(new(), new());
}