using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Data;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

/// <summary>
/// Finds best fitting mask. Thread safe.
/// </summary>
public class TileMaskSearcher
{
  public const int DEFAULT_WILDCARD_ID = -2;

  public readonly FrozenSet<TileMask> ExistingMasks;
  private readonly ImmutableArray<TileMask> items;
  private readonly IndexSearcher indexSearcher;
  private readonly int wildcardId;
  private readonly HashSet<int> connectionGroupTileIds;

  public TileMaskSearcher(
    IEnumerable<TileMask> tileMasks,
    HashSet<int>? connectionGroupTileIds = null,
    uint? wildcardId = null)
  {
    this.wildcardId = (int?)wildcardId ?? DEFAULT_WILDCARD_ID;
    this.connectionGroupTileIds = connectionGroupTileIds ?? [];
    ExistingMasks = tileMasks.ToFrozenSet();
    items = [.. tileMasks];

    indexSearcher = new(
      items.Length, [.. GetAssignedIndexes().Select(d => d.ToFrozenDictionary())], this.wildcardId);
  }

  /// <summary>
  /// If no field has a match returns random item
  /// </summary>
  public TileMask FindBestMatch(TileMask target)
  {
    if (ExistingMasks.Contains(target))
      return target;

    (int bestIndex, int _) = indexSearcher.Search(target);
    TileMask parsedTarget = new();
    if (bestIndex != -1)
    {
      var rawTileMask = items[bestIndex];
      parsedTarget = ParseTargetHitmask(target, rawTileMask);
    }

    if (ExistingMasks.Contains(parsedTarget))
      return parsedTarget;

    (int trimmedBestIndex, int _) = indexSearcher.Search(target);
    if (trimmedBestIndex == -1)
      return GetDefaultItem();
    return trimmedBestIndex != -1 ? items[trimmedBestIndex] : GetDefaultItem();
  }

  private TileMask ParseTargetHitmask(TileMask target, TileMask rawTileMask)
  {
    int GetHitMask(int target, int rawTileMask)
      => (target == rawTileMask
        || rawTileMask == wildcardId
        || (connectionGroupTileIds.Contains(target) && connectionGroupTileIds.Contains(rawTileMask)))
        ? rawTileMask : -1;

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

  private Dictionary<int, ImmutableArray<int>>[] GetAssignedIndexes()
  {
    var tileIdToItemIndexesTemp = new Dictionary<int, HashSet<int>>[8];
    for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      tileIdToItemIndexesTemp[fieldIndex] = [];

    AssignIndexes(tileIdToItemIndexesTemp);

    var result = new Dictionary<int, ImmutableArray<int>>[8];
    for (int i = 0; i < 8; i++)
    {
      result[i] = [];
      foreach (var kvp in tileIdToItemIndexesTemp[i])
        result[i][kvp.Key] = [.. kvp.Value];
    }
    return result;
  }

  private void AssignIndexes(Dictionary<int, HashSet<int>>[] tileIdToItemIndexesTemp)
  {
    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      var tileMask = items[itemIndex];
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var tileId = tileMask.GetTileIdByIndex(fieldIndex);
        var dict = tileIdToItemIndexesTemp[fieldIndex];
        AddSetValueToDict(dict, tileId, itemIndex);
        if (connectionGroupTileIds.Contains(tileId))
          foreach (var groupId in connectionGroupTileIds)
            AddSetValueToDict(dict, groupId, itemIndex);
      }
    }
  }

  private static void AddSetValueToDict<K, V>(Dictionary<K, HashSet<V>> dict, K key, V value)
    where K : notnull
  {
    if (!dict.TryGetValue(key, out var itemIndexes))
    {
      itemIndexes = [];
      dict[key] = itemIndexes;
    }
    itemIndexes.Add(value);
  }

  private TileMask GetDefaultItem()
    => items.Length > 0 ? items[0] : new();
}