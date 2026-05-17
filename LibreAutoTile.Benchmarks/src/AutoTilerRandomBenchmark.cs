using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser]
[ShortRunJob]
public class AutoTilerRandomBenchmark
{
  private const int MAX_TILE_ID = 100;

  [Params(100, 1_000, 2_000)]
  public int TileMaskCount;

  private readonly (Configuration.Models.Vector2, int)[] positionsWithIds100x100 = [];
  private AutoTiler autoTiler = null!;


  public AutoTilerRandomBenchmark()
  {
    positionsWithIds100x100 = Helper.GetPositionsWithIds(100, 100, MAX_TILE_ID);
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    autoTiler = new(1, Helper.GetIdsToRandomTileSearchers(MAX_TILE_ID, TileMaskCount));
  }

  [Benchmark]
  public void PlaceTile_SingleLayer_100x100_Random()
  {
    autoTiler.PlaceTiles(0, positionsWithIds100x100);
  }
}