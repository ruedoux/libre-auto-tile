using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

internal class IndexSearcher
{
  private const int TOP_SCORE = 3;
  private const int LOW_SCORE = 1;

  private readonly FrozenDictionary<int, ImmutableArray<int>>[] tileIdToItemIndexes;
  private readonly ImmutableArray<int>[] wildcardLists;
  private readonly uint[] itemIndexToSeenGeneration;
  private readonly int[] itemIndexToScore;
  private readonly int[] cornerWeight = new int[8];
  private readonly object searchLock = new();
  private uint currentGeneration = 0;

  public IndexSearcher(int itemCount, FrozenDictionary<int, ImmutableArray<int>>[] tileIdToItemIndexes, int wildcardId)
  {
    this.tileIdToItemIndexes = tileIdToItemIndexes;
    itemIndexToSeenGeneration = new uint[itemCount];
    itemIndexToScore = new int[itemCount];

    wildcardLists = new ImmutableArray<int>[8];
    for (int f = 0; f < 8; f++)
      wildcardLists[f] = tileIdToItemIndexes[f].TryGetValue(wildcardId, out var w) ? w : default;

    for (int i = 0; i < cornerWeight.Length; i++)
      cornerWeight[i] = TOP_SCORE;
  }

  // The con of this implementation is that best score can be assigned to multiple items
  // For simplicity it just picks last best score it finds
  public (int BestIndex, int BestScore) Search(TileMask target)
  {
    lock (searchLock)
    {
      cornerWeight[(int)TileMask.SurroundingDirection.TopLeft] = target.IsTopLeftConnected() ? TOP_SCORE : LOW_SCORE;
      cornerWeight[(int)TileMask.SurroundingDirection.TopRight] = target.IsTopRightConnected() ? TOP_SCORE : LOW_SCORE;
      cornerWeight[(int)TileMask.SurroundingDirection.BottomLeft] = target.IsBottomLeftConnected() ? TOP_SCORE : LOW_SCORE;
      cornerWeight[(int)TileMask.SurroundingDirection.BottomRight] = target.IsBottomRightConnected() ? TOP_SCORE : LOW_SCORE;

      IncrementGeneration();

      int bestIndex = -1, bestScore = 0;
      for (int fieldIndex = 0; fieldIndex < 8; fieldIndex++)
      {
        int tileId = target.GetTileIdByIndex(fieldIndex);

        ImmutableArray<int> itemIndexList =
          tileIdToItemIndexes[fieldIndex].TryGetValue(tileId, out var exactList)
            ? exactList
            : wildcardLists[fieldIndex];

        if (itemIndexList.IsDefaultOrEmpty)
          continue;

        int weightToAdd = cornerWeight[fieldIndex];
        for (int i = 0; i < itemIndexList.Length; i++)
        {
          int itemIndex = itemIndexList[i];
          if (itemIndexToSeenGeneration[itemIndex] != currentGeneration)
          {
            itemIndexToSeenGeneration[itemIndex] = currentGeneration;
            itemIndexToScore[itemIndex] = 0;
          }

          int updatedScore = itemIndexToScore[itemIndex] += weightToAdd;
          if (updatedScore > bestScore)
          {
            bestScore = updatedScore;
            bestIndex = itemIndex;
          }
        }
      }

      return (bestIndex, bestScore);
    }
  }

  private void IncrementGeneration()
  {
    currentGeneration++;
    if (currentGeneration == 0)
    {
      Array.Clear(itemIndexToSeenGeneration, 0, itemIndexToSeenGeneration.Length);
      currentGeneration = 1;
    }
  }
}