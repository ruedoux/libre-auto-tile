using BenchmarkDotNet.Attributes;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Benchmark;

[MemoryDiagnoser]
[ShortRunJob]
public class AutoTilerConcurrencyBenchmark
{
  private const int MAX_TILE_ID = 100;
  private const int TILE_MASK_COUNT = 1_000;
  private const int REGION_SIZE = 40;
  private const int WORKER_COUNT = 4;

  private readonly (Vector2 Position, int TileId)[][] regions =
  [
    BuildRegion(new(0, 0)),
    BuildRegion(new(60, 0)),
    BuildRegion(new(0, 60)),
    BuildRegion(new(60, 60))
  ];

  private AutoTiler sameLayerAutoTiler = null!;
  private AutoTiler differentLayersAutoTiler = null!;

  [GlobalSetup]
  public void GlobalSetup()
  {
    var tileSearchers = Helper.GetIdsToRandomTileSearchers(MAX_TILE_ID, TILE_MASK_COUNT);
    sameLayerAutoTiler = new(1, tileSearchers);
    differentLayersAutoTiler = new(WORKER_COUNT, tileSearchers);
  }

  [Benchmark]
  public void PlaceTiles_SameLayer_DisjointRegions_Parallel()
    => Parallel.For(0, WORKER_COUNT, workerIndex =>
      sameLayerAutoTiler.PlaceTiles(0, regions[workerIndex]));

  [Benchmark]
  public void PlaceTiles_DifferentLayers_SharedSearchers_Parallel()
    => Parallel.For(0, WORKER_COUNT, workerIndex =>
      differentLayersAutoTiler.PlaceTiles(workerIndex, regions[workerIndex]));

  private static (Vector2 Position, int TileId)[] BuildRegion(Vector2 offset)
  {
    Random random = new(123 + offset.X + offset.Y);
    (Vector2 Position, int TileId)[] tiles = new (Vector2 Position, int TileId)[REGION_SIZE * REGION_SIZE];

    for (int i = 0, x = 0; x < REGION_SIZE; x++)
      for (int y = 0; y < REGION_SIZE; y++)
        tiles[i++] = (new(offset.X + x, offset.Y + y), random.Next(-1, MAX_TILE_ID));

    return tiles;
  }
}
