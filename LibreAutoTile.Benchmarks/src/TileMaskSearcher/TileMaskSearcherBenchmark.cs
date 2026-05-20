using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;


[MemoryDiagnoser(displayGenColumns: false)]
public class TileMaskSearcherBenchmark
{
  [Params(1_000, 10_000)]
  public int TileMaskCount;

  [Params(1_000)]
  public int LookupCount;

  private readonly TileMask missMask = TileMask.GetZero();
  private TileMaskSearcher tileMaskSearcherNoCache = null!;
  private TileMaskSearcher tileMaskSearcherWithCache = null!;
  private TileMask[] items = null!;
  private TileMask[] uniqueMissTargets = null!;


  [IterationSetup]
  public void IterationSetup()
  {
    items = Helper.GetRandomTileMasks(TileMaskCount);
    uniqueMissTargets = Helper.GetRandomTileMasks(TileMaskCount);
    tileMaskSearcherNoCache = new(items, cacheSize: 0);
    tileMaskSearcherWithCache = new(items, cacheSize: 1024);
  }

  [Benchmark]
  public int FindBestMatch_ExactMaskFastPath()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherNoCache.FindBestMatch(items[0]).Bottom;
    return sink;
  }

  [Benchmark]
  public int FindBestMatch_SearchMiss()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherNoCache.FindBestMatch(missMask).Bottom;
    return sink;
  }

  [Benchmark]
  public int FindBestMatch_RepeatedMiss_WithCache()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherWithCache.FindBestMatch(missMask).Bottom;
    return sink;
  }

  [Benchmark]
  public int FindBestMatch_RepeatedMiss_NoCache()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherNoCache.FindBestMatch(missMask).Bottom;
    return sink;
  }

  [Benchmark]
  public int FindBestMatch_UniqueMisses_WithCache()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherWithCache.FindBestMatch(uniqueMissTargets[i]).Bottom;
    return sink;
  }

  [Benchmark]
  public int FindBestMatch_UniqueMisses_NoCache()
  {
    int sink = 0;
    for (int i = 0; i < LookupCount; i++)
      sink += tileMaskSearcherNoCache.FindBestMatch(uniqueMissTargets[i]).Bottom;
    return sink;
  }
}
