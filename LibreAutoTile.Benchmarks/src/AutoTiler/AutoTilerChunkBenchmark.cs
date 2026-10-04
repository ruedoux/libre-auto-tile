using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

/// <summary>
/// Measures the chunked dynamic backend against its two tuning knobs:
/// storage chunk size and free-chunk pool capacity.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[WarmupCount(4)]
[IterationCount(15)]
public class AutoTilerChunkBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  [Params(256, 512)]
  public int TileGridSize;

  [Params(32, 64, 128)]
  public int StorageChunkSize;

  [Params(0, 64)]
  public int ChunkPoolCapacity;

  private (Vector2, int)[] positionsWithIds = null!;
  private AutoTiler autoTiler = null!;

  static AutoTilerChunkBenchmark()
  {
    AUTO_TILE_CONFIGURATION = AutoTileConfiguration.LoadFromFile(
      Path.Combine(AppContext.BaseDirectory, AUTOTILE_EXAMPLE_PATH));
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    positionsWithIds = Helper.GetRandomChunk(TileGridSize, TileGridSize, minTileId: 0, maxTileId: 2);
    autoTiler = new(
      1,
      AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, 1024),
      StorageChunkSize,
      ChunkPoolCapacity);
  }

  [IterationSetup]
  public void IterationSetup()
    => autoTiler.Clear();

  [Benchmark]
  public void PlaceTiles()
    => autoTiler.PlaceTiles(0, positionsWithIds);
}
