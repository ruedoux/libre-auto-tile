using System.Collections.Frozen;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

internal class IndexSearcher(int itemCount)
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