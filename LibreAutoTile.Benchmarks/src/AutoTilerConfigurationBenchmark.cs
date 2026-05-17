using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser]
[ShortRunJob]
public class AutoTilerConfigurationBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  private readonly (Configuration.Models.Vector2, int)[] positionsWithIds100x100 = [];
  private AutoTiler autoTiler = null!;

  static AutoTilerConfigurationBenchmark()
  {
    AUTO_TILE_CONFIGURATION = AutoTileConfiguration.LoadFromFile(
      Path.Combine(AppContext.BaseDirectory, AUTOTILE_EXAMPLE_PATH));
  }

  public AutoTilerConfigurationBenchmark()
  {
    positionsWithIds100x100 = Helper.GetPositionsWithIds(100, 100, 1);
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    autoTiler = new(1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION));
  }

  [Benchmark]
  public void PlaceTile_SingleLayer_100x100_Configuration()
  {
    autoTiler.PlaceTiles(0, positionsWithIds100x100);
  }
}