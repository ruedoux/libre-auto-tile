using BenchmarkDotNet.Attributes;
using Qwaitumin.AutoTile;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.Autotile.Benchmark;


static class TileMaskSearcherItemSetup
{
  const int MAX_TILE_ID = 1000;

  public static List<(TileMask TileMask, TileAtlas tileAtlas)> GetItems(int n)
  {
    List<(TileMask TileMask, TileAtlas tileAtlas)> items = [];
    Random random = new();
    for (int i = 0; i < n; i++)
    {
      TileMask tileMask = new(
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID),
        random.Next(0, MAX_TILE_ID)
      );
      Vector2 atlasPosition = new(i, i);
      items.Add(new(tileMask, new(atlasPosition, "")));
    }

    return items;
  }
}

[MemoryDiagnoser]
[ShortRunJob]
public class TileMaskSearcherBenchmark
{

  [Params(1_000, 10_000, 100_000)]
  public int N;

  TileMaskSearcher tileMaskSearcher = null!;
  (TileMask TileMask, TileAtlas tileAtlas) randomTileMask;

  [GlobalSetup]
  public void GlobalSetup()
  {
    List<(TileMask TileMask, TileAtlas tileAtlas)> items = TileMaskSearcherItemSetup.GetItems(N);
    tileMaskSearcher = new(items);
    randomTileMask = items[new Random().Next(0, items.Count)];
  }

  [Benchmark]
  public (TileMask TileMask, TileAtlas tileAtlas) LookupExisting()
    => tileMaskSearcher.FindBestMatch(randomTileMask.TileMask);

  [Benchmark]
  public (TileMask TileMask, TileAtlas tileAtlas) LookupClosest()
    => tileMaskSearcher.FindBestMatch(new TileMask());
}