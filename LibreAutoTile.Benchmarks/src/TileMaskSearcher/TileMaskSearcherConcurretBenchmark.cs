using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;


[MemoryDiagnoser(displayGenColumns: false)]
public class TileMaskSearcherConcurrentBenchmark
{
  [Params(1_000)]
  public int TileMaskCount;

  [Params(2, 4, 8)]
  public int WorkerCount;

  [Params(1_000)]
  public int LookupCount;

  private TileMaskSearcher tileMaskSearcher = null!;
  private TileMask[] searchSpace = null!;

  [GlobalSetup]
  public void GlobalSetup()
  {
    searchSpace = Helper.GetRandomTileMasks(TileMaskCount, maxTileId: 1);
    tileMaskSearcher = new(searchSpace, cacheSize: 0);
  }

  [Benchmark]
  public long FindBestMatch_Sequential()
    => ComputeRangeSum(0, LookupCount);

  [Benchmark]
  public long FindBestMatch_Parallel()
  {
    long total = 0;
    int lookupCountPerWorker = Math.DivRem(LookupCount, WorkerCount, out int remainder);
    Parallel.For(
      0,
      WorkerCount,
      () => 0L,
      (workerIndex, _, local) =>
      {
        int start = workerIndex * lookupCountPerWorker + Math.Min(workerIndex, remainder);
        int count = lookupCountPerWorker + (workerIndex < remainder ? 1 : 0);
        return local + ComputeRangeSum(start, count);
      },
      local => Interlocked.Add(ref total, local));

    return total;
  }

  private long ComputeRangeSum(int start, int count)
  {
    long sink = 0;
    for (int i = start; i < start + count; i++)
      sink += tileMaskSearcher.FindBestMatch(TileMask.GetZero()).Bottom;
    return sink;
  }
}
