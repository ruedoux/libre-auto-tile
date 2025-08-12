using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;

namespace Qwaitumin.LibreAutoTile.Benchmark;


static class TileMaskSearcherItemSetup
{
  const int MAX_TILE_ID = 1000;

  public static List<(TileMask TileMask, TileAtlas tileAtlas)> GetItems(int n)
  {
    List<(TileMask TileMask, TileAtlas tileAtlas)> items = [];
    Random random = new();
    for (int i = 0; i < n; i++)
    {
      TileMask tileMask = new(
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID)
      );
      Vector2 atlasPosition = new(i, i);
      items.Add(new(tileMask, new(atlasPosition, "")));
    }

    return items;
  }
}

[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherBenchmark
{

  [Params(1_000, 10_000)]
  public int N;

  TileMaskSearcher tileMaskSearcher = null!;

  List<(TileMask TileMask, TileAtlas tileAtlas)> items = [];
  List<(TileMask TileMask, TileAtlas tileAtlas)> itemsToMatch = [];
  (TileMask TileMask, TileAtlas tileAtlas) randomTileMask;

  [GlobalSetup]
  public void GlobalSetup()
  {
    items = TileMaskSearcherItemSetup.GetItems(N);
    itemsToMatch = TileMaskSearcherItemSetup.GetItems(N);
    tileMaskSearcher = new(items, []);
    randomTileMask = items[new Random(123).Next(0, items.Count)];
  }

  /// <summary>
  /// Find a singular existing 1:1 match of a TileMask, basically a dict lookup
  /// Best case scenario.
  /// </summary>
  [Benchmark]
  public void FindBestMatchSingle_BestCaseScenario()
    => tileMaskSearcher.FindBestMatch(randomTileMask.TileMask);

  /// <summary>
  /// Find a singular closest match of a TileMask, no 1:1 match
  /// Worst case scenario.
  /// </summary>
  [Benchmark]
  public void FindBestMatchSingle_WorstCaseScenario()
    => tileMaskSearcher.FindBestMatch(new TileMask());

  /// <summary>
  /// Find a batch of (N) existing matches of a TileMask, 1:1 match
  /// Best case scenario.
  /// </summary>
  [Benchmark]
  public void FindBestMatchBatch_BestCaseScenario()
  {
    foreach (var item in items)
      tileMaskSearcher.FindBestMatch(item.TileMask);
  }

  /// <summary>
  /// Find a batch of (N) closest matches of a TileMask, no 1:1 match
  /// Worst case scenario.
  /// </summary>
  [Benchmark]
  public void FindBestMatchBatch_WorstCaseScenario()
  {
    foreach (var item in itemsToMatch)
      tileMaskSearcher.FindBestMatch(item.TileMask);
  }
}