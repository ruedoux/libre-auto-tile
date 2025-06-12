using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests;


[SimpleTestClass]
public class AutoTileDrawerTest
{
  const int TILE_ID = 0;

  private static AutoTiler GetMockedAutoTiler(uint layerCount)
  {
    TileMaskSearcher tileMaskSearcher = new([(new(), new())]);
    return new(layerCount, new() { { TILE_ID, tileMaskSearcher } });
  }

  private static (Vector2, int)[] GetMockedPositionsToTileIds(
    Vector2[] positions, int tileId)
  {
    List<(Vector2, int)> positionsToTileIds = [];
    foreach (var position in positions)
      positionsToTileIds.Add(new(position, tileId));

    return [.. positionsToTileIds];
  }

  [SimpleTestMethod]
  public void DrawTiles_ShouldDrawTiles_WhenMockedDrawer()
  {
    // Given
    Vector2[] positions = GetVector2Rectangle(Vector2.Zero, new(16, 16));
    AutoTiler autoTiler = GetMockedAutoTiler(1);
    MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTiles(0, GetMockedPositionsToTileIds(positions, TILE_ID));

    // Then
    foreach (var position in positions)
      Assertions.AssertEqual(
        TILE_ID, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
  }

  [SimpleTestMethod]
  public void DrawTilesAsync_ShouldDrawTiles_WhenMockedDrawer()
  {
    // Given
    int tileId = 0;
    Vector2[] positions = GetVector2Rectangle(Vector2.Zero, new(16, 16));

    AutoTiler autoTiler = GetMockedAutoTiler(1);
    MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTilesAsync(0, GetMockedPositionsToTileIds(positions, tileId));

    // Then
    Assertions.AssertAwaitAtMost(100, () => // 100ms is more than enough for 16x16 tiles
    {
      foreach (var position in positions)
        Assertions.AssertEqual(
          tileId, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
    });
  }

  [SimpleTestMethod]
  public void DrawTilesAsync_ShouldDrawTiles_WhenMockedDrawerAndWait()
  {
    // Given
    int tileId = 0;
    Vector2[] positions = GetVector2Rectangle(Vector2.Zero, new(16, 16));

    AutoTiler autoTiler = GetMockedAutoTiler(1);
    MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTilesAsync(0, GetMockedPositionsToTileIds(positions, tileId));
    autoTileDrawer.Wait();

    // Then
    foreach (var position in positions)
      Assertions.AssertEqual(
        tileId, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
  }

  public static Vector2[] GetVector2Rectangle(Vector2 at, Vector2 size)
  {
    Vector2[] arr = new Vector2[size.X * size.Y];

    int index = 0;
    for (int x = 0; x < size.X; x++)
      for (int y = 0; y < size.Y; y++)
        arr[index++] = at + new Vector2(x, y);

    return arr;
  }
}


class MockedTileMapDrawer : ITileMapDrawer
{
  public Dictionary<Vector2, TileData>[] Data;

  public MockedTileMapDrawer(int layerCount)
  {
    Data = new Dictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
      Data[layer] = [];
  }

  public void Clear()
  {
    foreach (var dataLayer in Data)
      dataLayer.Clear();
  }

  public void ClearTiles(int layer, Vector2[] positions)
  {
    foreach (var position in positions)
      Data[layer].Remove(position);
  }

  public void DrawTiles(int tileLayer, KeyValuePair<Vector2, TileData>[] positionsToTileData)
  {
    foreach (var (position, tileData) in positionsToTileData)
      Data[tileLayer][position] = tileData;
  }
}