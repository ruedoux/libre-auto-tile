using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;


/// <summary>
/// Finds best fitting mask. Thread safe.
/// </summary>
public class TileMaskSearcher
{
  public const int DEFAULT_WILDCARD_ID = -2;

  private readonly FrozenSet<TileMask> existingMasks;
  private readonly ImmutableArray<TileMask> items;
  private readonly IndexSearcher indexSearcher;

  public TileMaskSearcher(
    IEnumerable<TileMask> tileMasks,
    HashSet<int>? connectionGroupTileIds = null,
    uint? wildcardId = null)
  {
    items = [.. tileMasks.Distinct()];
    existingMasks = items.ToFrozenSet();
    indexSearcher = new(
      items.Length,
      [.. BuildTileIdToItemIndexes(connectionGroupTileIds ?? []).Select(static d => d.ToFrozenDictionary())],
      (int?)wildcardId ?? DEFAULT_WILDCARD_ID);
  }

  private readonly TileMaskCache cache = new(1024);

  /// <summary>
  /// If no field has a match returns random item
  /// </summary>
  public TileMask FindBestMatch(TileMask target)
  {
    if (existingMasks.Contains(target))
      return target;

    if (cache.TryGet(target, out var cached))
      return cached;

    int bestIndex = indexSearcher.Search(target);
    var result = bestIndex >= 0 ? items[bestIndex] : items.Length > 0 ? items[0] : new();

    cache.Set(target, result);
    return result;
  }

  private Dictionary<int, ImmutableArray<int>>[] BuildTileIdToItemIndexes(HashSet<int> connectionGroupTileIds)
  {
    Dictionary<int, HashSet<int>>[] indexes = [[], [], [], [], [], [], [], []];

    for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
    {
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        var dict = indexes[fieldIndex];
        int tileId = items[itemIndex].GetTileIdByIndex(fieldIndex);
        AddItemIndex(dict, tileId, itemIndex);
        if (!connectionGroupTileIds.Contains(tileId))
          continue;

        foreach (int groupId in connectionGroupTileIds)
          AddItemIndex(dict, groupId, itemIndex);
      }
    }

    return [.. indexes.Select(static dict =>
      dict.ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value.ToImmutableArray()))];
  }

  private static void AddItemIndex(Dictionary<int, HashSet<int>> dict, int key, int value)
  {
    ref var values = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    (values ??= []).Add(value);
  }
}
