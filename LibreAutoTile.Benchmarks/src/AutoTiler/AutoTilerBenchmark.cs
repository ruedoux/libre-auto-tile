using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[WarmupCount(4)]
[IterationCount(15)]
public class AutoTilerBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  [Params(256, 512)]
  public int ChunkSize;

  [Params(false, true)]
  public bool UseStaticMap;

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
    positionsWithIds = Helper.GetRandomChunk(ChunkSize, ChunkSize, minTileId: 0, maxTileId: 2);
  }

  [IterationSetup]
  public void IterationSetup()
  {
    Vector2 mapSize = UseStaticMap
      ? new(ChunkSize, ChunkSize)
      : default;
    autoTilerNoCache = new(
      1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, 0), mapSize);
    autoTilerWithCache = new(
      1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, 1024), mapSize);
  }

  [Benchmark]
  public void PlaceTiles_NoCache()
    => autoTilerNoCache.PlaceTiles(0, positionsWithIds);

  [Benchmark]
  public void PlaceTiles_WithCache()
    => autoTilerWithCache.PlaceTiles(0, positionsWithIds);
}
