using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Benchmark;



internal static class Helper
{
  public static TileMask[] GetRandomTileMasks(
    int n, int maxTileId = 1000)
  {
    TileMask[] items = new TileMask[n];
    Random random = new();
    for (int i = 0; i < n; i++)
    {
      TileMask tileMask = new(
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId),
        random.Next(-1, maxTileId)
      );
      items[i] = tileMask;
    }

    return items;
  }

  public static TileAtlas[] GetRandomTileAtlases(int n)
  {
    TileAtlas[] items = new TileAtlas[n];
    for (int x = 0; x < n; x++)
      items[x] = new(new(x, 0), "");
    return items;
  }

  public static (Vector2, int)[] GetPositionsWithIds(
    int xRange, int yRange, int maxTileId = 1000)
  {
    Random random = new();
    List<(Vector2, int)> positionsWithIds = [];
    for (int x = 0; x < xRange; x++)
      for (int y = 0; y < yRange; y++)
        positionsWithIds.Add(new(new(x, y), random.Next(-1, maxTileId)));

    return [.. positionsWithIds];
  }

  public static Dictionary<int, TileSearcher> GetIdsToRandomTileSearchers(
    int maxTileId, int tileMaskCount)
  {
    Dictionary<int, TileSearcher> idsToTileMaskSearchers = [];
    for (int tileId = 0; tileId < maxTileId; tileId++)
    {
      var tileMasks = GetRandomTileMasks(tileMaskCount, maxTileId);
      var tileAtlases = GetRandomTileAtlases(tileMaskCount);
      TileMaskSearcher tileMaskSearcher = new(tileMasks);
      TileAtlasResolver tileAtlasResolver = new(tileMasks.Zip(tileAtlases, (a, b) => (a, b)));
      idsToTileMaskSearchers[tileId] = new(tileMaskSearcher, tileAtlasResolver);
    }

    return idsToTileMaskSearchers;
  }

  public static TileMask MutateTileMask(TileMask tileMask)
  {
    Random random = new();
    return new(
      topLeft: random.Next(),
      top: tileMask.Top,
      topRight: tileMask.TopRight,
      right: tileMask.Right,
      bottomRight: tileMask.BottomRight,
      bottom: tileMask.Bottom,
      bottomLeft: tileMask.BottomLeft,
      left: tileMask.Left);
  }
}
