using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherConcurrencyBenchmark
{
  private const int TILE_MASK_COUNT = 10_000;
  private const int WORKER_COUNT = 4;

  private TileMaskSearcher tileMaskSearcher = null!;
  private TileMask[][] workerItemsToMatch = [];
  private TileMask[] sinks = [];

  [GlobalSetup]
  public void GlobalSetup()
  {
    tileMaskSearcher = new(Helper.GetRandomTileMasks(TILE_MASK_COUNT));
    workerItemsToMatch = new TileMask[WORKER_COUNT][];
    sinks = new TileMask[WORKER_COUNT];

    for (int workerIndex = 0; workerIndex < WORKER_COUNT; workerIndex++)
      workerItemsToMatch[workerIndex] = Helper.GetRandomTileMasks(TILE_MASK_COUNT / WORKER_COUNT);
  }

  [Benchmark]
  public TileMask FindBestMatchBatch_Random_WorstCaseScenario_Parallel()
  {
    Parallel.For(0, WORKER_COUNT, workerIndex =>
    {
      foreach (var item in workerItemsToMatch[workerIndex])
        sinks[workerIndex] = tileMaskSearcher.FindBestMatch(item);
    });

    return sinks[0];
  }
}
