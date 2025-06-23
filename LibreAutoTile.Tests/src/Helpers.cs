using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Tests;

public static class Helpers
{
  public const int TILE_ID = 0;

  public static Vector2[] GetVector2Rectangle(Vector2 at, Vector2 size)
  {
    Vector2[] arr = new Vector2[size.X * size.Y];

    int index = 0;
    for (int x = 0; x < size.X; x++)
      for (int y = 0; y < size.Y; y++)
        arr[index++] = at + new Vector2(x, y);

    return arr;
  }

  public static (Vector2, int)[] GetMockedPositionsToTileIds(Vector2[] positions)
  {
    List<(Vector2, int)> positionsToTileIds = [];
    foreach (var position in positions)
      positionsToTileIds.Add(new(position, TILE_ID));

    return [.. positionsToTileIds];
  }
}
