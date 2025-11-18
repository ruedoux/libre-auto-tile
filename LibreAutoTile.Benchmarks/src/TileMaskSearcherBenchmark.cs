using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;

namespace Qwaitumin.LibreAutoTile.Benchmark;


[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherBenchmark
{

  [Params(1_000, 10_000)]
  public int TileMaskCount;

  private TileMaskSearcher tileMaskSearcher = null!;

  private TileMask[] items = [];
  private TileMask[] itemsToMatch = [];
  private TileMask randomTileMask;

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
  public void FindBestMatchSingle_BestCaseScenario()
    => tileMaskSearcher.FindBestMatch(randomTileMask);

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
      tileMaskSearcher.FindBestMatch(item);
  }

  /// <summary>
  /// Find a batch of (N) closest matches of a TileMask, no 1:1 match
  /// Worst case scenario.
  /// </summary>
  [Benchmark]
  public void FindBestMatchBatch_WorstCaseScenario()
  {
    foreach (var item in itemsToMatch)
      tileMaskSearcher.FindBestMatch(item);
  }
}