using BenchmarkDotNet.Attributes;
using LibreAutoTile.Configuration;
using LibreAutoTile.Configuration.Models;
using LibreAutoTile.Tiling;

namespace LibreAutoTile.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[WarmupCount(4)]
[IterationCount(15)]
public class AutoTilerBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  [Params(256, 512)]
  public int TileGridSize;

  private (Vector2, int)[] positionsWithIds = null!;
  private AutoTiler autoTilerNoCache = null!;
  private AutoTiler autoTilerWithCache = null!;

  static AutoTilerBenchmark()
  {
    AUTO_TILE_CONFIGURATION = AutoTileConfiguration.LoadFromFile(
      Path.Combine(AppContext.BaseDirectory, AUTOTILE_EXAMPLE_PATH));
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    positionsWithIds = Helper.GetRandomChunk(TileGridSize, TileGridSize, minTileId: 0, maxTileId: 2);

    // Constructed once so measured iterations reflect steady-state (warm chunk pool +
    // grown reusable dirty-position set), not first-call setup cost.
    autoTilerNoCache = new(
      1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, 0));
    autoTilerWithCache = new(
      1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, 1024));
  }

  [IterationSetup]
  public void IterationSetup()
  {
    autoTilerNoCache.Clear();
    autoTilerWithCache.Clear();
  }

  [Benchmark]
  public void PlaceTiles_NoCache()
    => autoTilerNoCache.PlaceTiles(0, positionsWithIds);

  [Benchmark]
  public void PlaceTiles_WithCache()
    => autoTilerWithCache.PlaceTiles(0, positionsWithIds);
}
