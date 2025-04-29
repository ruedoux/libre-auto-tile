using System.Collections.Frozen;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile;

public readonly struct TileAtlas
{
  public const string DEFAULT_FILE = "<NO FILE SPECIFIED>";
  public readonly Vector2 Position { get; init; } = default;
  public readonly string ImageFileName { get; init; } = DEFAULT_FILE;

  public TileAtlas() { }

  public TileAtlas(Vector2 position, string imageFileName) : this()
  {
    Position = position;
    ImageFileName = imageFileName;
  }

  public override readonly string ToString()
    => $"({Position}, {ImageFileName})";
}


// TODO this needs refactor
public class TileMaskSearcher
{
  private const int DIMENSION_COUNT = 8;
  private readonly List<(TileMask TileMask, TileAtlas TileAtlas)> items;
  public readonly FrozenDictionary<TileMask, TileAtlas> ExistingMasks;

  private readonly FrozenDictionary<int, List<int>>[] CornerDictionaries;

  public TileMaskSearcher(List<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    Dictionary<TileMask, TileAtlas> existingMasks = [];
    List<(TileMask TileMask, TileAtlas TileAtlas)> sanitizedPoints = [];

    foreach (var item in items)
      if (existingMasks.TryAdd(item.TileMask, item.TileAtlas))
        sanitizedPoints.Add(item);

    ExistingMasks = existingMasks.ToFrozenDictionary();
    this.items = sanitizedPoints;

    CornerDictionaries = ConstructCornerDictionaries(items);
  }

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    if (ExistingMasks.TryGetValue(target, out var fastLookup))
      return (target, fastLookup);

    int[] itemIndexToMatchCount = new int[items.Count];
    int bestMatchCount = -1;
    int bestMatchIndex = -1;

    var tileMaskArray = target.ToArray();

    for (int i = 0; i < tileMaskArray.Length; i++)
    {
      bestMatchIndex = ProcessDimension(
        CornerDictionaries[i],
        tileMaskArray[i],
        itemIndexToMatchCount,
        ref bestMatchCount,
        bestMatchIndex);
      if (bestMatchCount == DIMENSION_COUNT) return items[bestMatchIndex];
    }

    return bestMatchIndex >= 0 ? items[bestMatchIndex] : new(new(), new());
  }

  private static FrozenDictionary<int, List<int>>[] ConstructCornerDictionaries(
    List<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    var cornerDictionariesTemp = new Dictionary<int, List<int>>[8];
    var cornerDictionaries = new FrozenDictionary<int, List<int>>[8];

    for (int i = 0; i < cornerDictionaries.Length; i++)
      cornerDictionariesTemp[i] = [];

    for (int i = 0; i < items.Count; i++)
    {
      var tileMask = items[i].TileMask;
      AddItemToDictionary(cornerDictionariesTemp[0], tileMask.TopLeft, i);
      AddItemToDictionary(cornerDictionariesTemp[1], tileMask.Top, i);
      AddItemToDictionary(cornerDictionariesTemp[2], tileMask.TopRight, i);
      AddItemToDictionary(cornerDictionariesTemp[3], tileMask.Right, i);
      AddItemToDictionary(cornerDictionariesTemp[4], tileMask.BottomRight, i);
      AddItemToDictionary(cornerDictionariesTemp[5], tileMask.Bottom, i);
      AddItemToDictionary(cornerDictionariesTemp[6], tileMask.BottomLeft, i);
      AddItemToDictionary(cornerDictionariesTemp[7], tileMask.Left, i);
    }

    for (int i = 0; i < cornerDictionaries.Length; i++)
      cornerDictionaries[i] = cornerDictionariesTemp[i].ToFrozenDictionary();

    return cornerDictionaries;
  }

  private static void AddItemToDictionary(
    Dictionary<int, List<int>> cornerDictionary, int tileId, int itemIndex)
  {
    if (!cornerDictionary.TryGetValue(tileId, out var list))
    {
      list = [];
      cornerDictionary[tileId] = list;
    }
    list.Add(itemIndex);
  }

  private static int ProcessDimension(
    FrozenDictionary<int, List<int>> cornerDictionary,
    int targetTileId,
    int[] itemIndexToMatchCount,
    ref int bestMatchCount,
    int currentBestIndex)
  {
    if (cornerDictionary.TryGetValue(targetTileId, out var list))
    {
      foreach (var itemIndex in list)
      {
        itemIndexToMatchCount[itemIndex]++;
        if (itemIndexToMatchCount[itemIndex] > bestMatchCount)
        {
          bestMatchCount = itemIndexToMatchCount[itemIndex];
          currentBestIndex = itemIndex;

          if (bestMatchCount == DIMENSION_COUNT)
            break;
        }
      }
    }
    return currentBestIndex;
  }
}