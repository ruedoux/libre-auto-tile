using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;


[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherRandomizedBenchmark
{
  [Params(1_000, 10_000)]
  public int TileMaskCount;

  private TileMaskSearcher tileMaskSearcher = null!;

  private TileMask[] items = [];
  private TileMask[] itemsToMatch = [];
  private TileMask randomTileMask;
  private TileMask sink;

  [GlobalSetup]
  public void GlobalSetup()
  {
    items = Helper.GetRandomTileMasks(TileMaskCount);
    itemsToMatch = Helper.GetRandomTileMasks(TileMaskCount);
    tileMaskSearcher = new(items);
    randomTileMask = items[new Random(123).Next(0, items.Length)];
  }

  /// <summary>
  /// Find a singular existing 1:1 match of a TileMask, basically a dict lookup
  /// Best case scenario.
  /// </summary>
  [Benchmark]
  public TileMask FindBestMatchSingle_Random_BestCaseScenario()
    => tileMaskSearcher.FindBestMatch(randomTileMask);

  /// <summary>
  /// Find a singular closest match of a TileMask, no 1:1 match
  /// Worst case scenario.
  /// </summary>
  [Benchmark]
  public TileMask FindBestMatchSingle_Random_WorstCaseScenario()
    => tileMaskSearcher.FindBestMatch(new TileMask());

  /// <summary>
  /// Find a batch of (N) existing matches of a TileMask, 1:1 match
  /// Best case scenario.
  /// </summary>
  [Benchmark]
  public TileMask FindBestMatchBatch_Random_BestCaseScenario()
  {
    foreach (var item in items)
      sink = tileMaskSearcher.FindBestMatch(item);
    return sink;
  }

  /// <summary>
  /// Find a batch of (N) closest matches of a TileMask, no 1:1 match
  /// Worst case scenario.
  /// </summary>
  [Benchmark]
  public TileMask FindBestMatchBatch_Random_WorstCaseScenario()
  {
    foreach (var item in itemsToMatch)
      sink = tileMaskSearcher.FindBestMatch(item);
    return sink;
  }
}
