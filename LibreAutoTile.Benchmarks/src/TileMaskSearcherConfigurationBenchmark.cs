using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;


[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherConfigurationBenchmark
{
  private const string AUTOTILE_EXAMPLE_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  private static readonly AutoTileConfiguration AUTO_TILE_CONFIGURATION;

  private (TileSearcher Searcher, TileMask TileMask)[] configBestCases = null!;
  private (TileSearcher Searcher, TileMask TileMask)[] configWorstCases = null!;

  private TileMask sink;

  static TileMaskSearcherConfigurationBenchmark()
  {
    AUTO_TILE_CONFIGURATION = AutoTileConfiguration.LoadFromFile(
      Path.Combine(AppContext.BaseDirectory, AUTOTILE_EXAMPLE_PATH));
  }

  [GlobalSetup]
  public void GlobalSetup()
  {
    var tileIdToSearcher = AutoTileConfigurationExtractor
      .BuildTileIdToTileMaskSearcher(AUTO_TILE_CONFIGURATION);

    configBestCases = [.. AUTO_TILE_CONFIGURATION.TileDefinitions
      .SelectMany(kvp =>
      {
        var searcher = tileIdToSearcher[(int)kvp.Key];
        return AutoTileConfigurationExtractor.GetItems(kvp.Value)
          .Select(item => (searcher, item.TileMask));
      })];

    configWorstCases = [.. AUTO_TILE_CONFIGURATION.TileDefinitions
      .SelectMany(kvp =>
      {
        var searcher = tileIdToSearcher[(int)kvp.Key];
        return AutoTileConfigurationExtractor.GetItems(kvp.Value)
          .Select(item => (searcher, Helper.MutateTileMask(item.TileMask)));
      })];
  }

  [Benchmark]
  public TileMask FindBestMatchAll_Configuration_BestCaseScenario()
  {
    foreach (var (searcher, tileMask) in configBestCases)
      sink = searcher.FindBestMatch(tileMask).TileMask;
    return sink;
  }

  [Benchmark]
  public TileMask FindBestMatchAll_Configuration_WorstCaseScenario()
  {
    foreach (var (searcher, tileMask) in configWorstCases)
      sink = searcher.FindBestMatch(tileMask).TileMask;
    return sink;
  }
}
