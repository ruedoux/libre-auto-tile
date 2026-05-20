using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;

internal static class Helper
{
  private const int DEFAULT_SEED = 1729;

  public static TileMask[] GetRandomTileMasks(
    int n, int minTileId = -1, int maxTileId = 1000, int seed = DEFAULT_SEED)
  {
    TileMask[] items = new TileMask[n];
    Random random = new(seed);
    for (int i = 0; i < n; i++)
    {
      TileMask tileMask = new(
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId),
        random.Next(minTileId, maxTileId)
      );
      items[i] = tileMask;
    }

    return items;
  }

  public static (Vector2 Position, int TileId)[] GetRandomChunk(
    int xRange, int yRange, int minTileId = -1, int maxTileId = 1000, int seed = DEFAULT_SEED)
  {
    Random random = new(seed);
    List<(Vector2, int)> positionsWithIds = [];
    for (int x = 0; x < xRange; x++)
      for (int y = 0; y < yRange; y++)
        positionsWithIds.Add(new(new(x, y), random.Next(minTileId, maxTileId)));

    return [.. positionsWithIds];
  }
}
