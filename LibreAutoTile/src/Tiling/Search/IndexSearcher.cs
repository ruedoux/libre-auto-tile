using System.Collections.Frozen;
using System.Collections.Immutable;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

// TODO interface, should have possibility of picking either square or hexagonal tiles
public sealed class IndexSearcher
{
  private const int TOP_SCORE = 3;
  private const int LOW_SCORE = 1;
  private const int EMPTY_TILE_ID = -1;

  private readonly FrozenDictionary<int, ImmutableArray<int>>[] tileIdToItemIndexes;
  private readonly ImmutableArray<int>[] emptyTileLists;
  private readonly ImmutableArray<int>[] wildcardLists;
  private readonly ThreadLocal<SearchScratch> scratch;

  public IndexSearcher(
    int itemCount,
    FrozenDictionary<int, ImmutableArray<int>>[] tileIdToItemIndexes,
    int wildcardId)
  {
    this.tileIdToItemIndexes = tileIdToItemIndexes;
    scratch = new(() => new(itemCount));

    emptyTileLists = new ImmutableArray<int>[8];
    wildcardLists = new ImmutableArray<int>[8];
    for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
    {
      emptyTileLists[fieldIndex] =
        tileIdToItemIndexes[fieldIndex].TryGetValue(EMPTY_TILE_ID, out var emptyList) ? emptyList : default;
      wildcardLists[fieldIndex] =
        tileIdToItemIndexes[fieldIndex].TryGetValue(wildcardId, out var wildcardList) ? wildcardList : default;
    }
  }

  // The con of this implementation is that best score can be assigned to multiple items.
  // For simplicity it just picks the last item with the best score.
  public int Search(TileMask target)
  {
    bool isTopLeftConnected = target.IsTopLeftConnected();
    bool isTopRightConnected = target.IsTopRightConnected();
    bool isBottomLeftConnected = target.IsBottomLeftConnected();
    bool isBottomRightConnected = target.IsBottomRightConnected();

    SearchScratch localScratch = scratch.Value!;
    localScratch.IncrementGeneration();

    int bestIndex = -1;
    int bestScore = 0;
    for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
    {
      int tileId = target.GetTileIdByIndex(fieldIndex);

      ImmutableArray<int> itemIndexList =
        tileIdToItemIndexes[fieldIndex].TryGetValue(tileId, out var exactList)
          ? exactList
          : !wildcardLists[fieldIndex].IsDefaultOrEmpty
            ? wildcardLists[fieldIndex]
            : emptyTileLists[fieldIndex];

      if (itemIndexList.IsDefaultOrEmpty)
        continue;

      int weightToAdd = fieldIndex switch
      {
        (int)TileMask.SurroundingDirection.TopLeft => isTopLeftConnected ? TOP_SCORE : LOW_SCORE,
        (int)TileMask.SurroundingDirection.TopRight => isTopRightConnected ? TOP_SCORE : LOW_SCORE,
        (int)TileMask.SurroundingDirection.BottomLeft => isBottomLeftConnected ? TOP_SCORE : LOW_SCORE,
        (int)TileMask.SurroundingDirection.BottomRight => isBottomRightConnected ? TOP_SCORE : LOW_SCORE,
        _ => TOP_SCORE
      };

      for (int i = 0; i < itemIndexList.Length; i++)
      {
        int itemIndex = itemIndexList[i];
        if (localScratch.ItemIndexToSeenGeneration[itemIndex] != localScratch.CurrentGeneration)
        {
          localScratch.ItemIndexToSeenGeneration[itemIndex] = localScratch.CurrentGeneration;
          localScratch.ItemIndexToScore[itemIndex] = 0;
        }

        int updatedScore = localScratch.ItemIndexToScore[itemIndex] += weightToAdd;
        if (updatedScore > bestScore)
        {
          bestScore = updatedScore;
          bestIndex = itemIndex;
        }
      }
    }

    return bestIndex;
  }

  private sealed class SearchScratch(int itemCount)
  {
    public readonly uint[] ItemIndexToSeenGeneration = new uint[itemCount];
    public readonly int[] ItemIndexToScore = new int[itemCount];
    public uint CurrentGeneration;

    public void IncrementGeneration()
    {
      CurrentGeneration++;
      if (CurrentGeneration == 0)
      {
        Array.Clear(ItemIndexToSeenGeneration, 0, ItemIndexToSeenGeneration.Length);
        CurrentGeneration = 1;
      }
    }
  }
}