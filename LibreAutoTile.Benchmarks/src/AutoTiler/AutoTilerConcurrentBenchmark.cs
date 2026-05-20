using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[WarmupCount(4)]
[IterationCount(15)]
public class AutoTilerConcurrentBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  [Params(4, 8)]
  public int LayerCount;

  [Params(false, true)]
  public bool UseStaticMap;

  [Params(256)]
  public int ChunkSize;

  private AutoTiler autoTiler = null!;
  private (Vector2, int)[][] positionsWithIdsArray = null!;

  static AutoTilerConcurrentBenchmark()
  {
    AUTO_TILE_CONFIGURATION = AutoTileConfiguration.LoadFromFile(
      Path.Combine(AppContext.BaseDirectory, AUTOTILE_EXAMPLE_PATH));
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    positionsWithIdsArray = new (Vector2, int)[LayerCount][];
    for (int i = 0; i < LayerCount; i++)
      positionsWithIdsArray[i] = Helper.GetRandomChunk(ChunkSize, ChunkSize, maxTileId: 2);
  }

  [IterationSetup]
  public void IterationSetup()
  {
    Vector2 mapSize = UseStaticMap
      ? new(ChunkSize, ChunkSize)
      : default;

    autoTiler = new(
      LayerCount, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION, cacheSize: 1024), mapSize);
  }

  [Benchmark]
  public void PlaceTiles_DifferentLayers_Sequential()
  {
    for (int i = 0; i < LayerCount; i++)
      autoTiler.PlaceTiles(i, positionsWithIdsArray[i]);
  }

  [Benchmark]
  public void PlaceTiles_DifferentLayers_Parallel()
    => Parallel.For(0, LayerCount, layer =>
      autoTiler.PlaceTiles(layer, positionsWithIdsArray[layer]));
}
