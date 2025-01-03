using System.Numerics;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class AutoTileDrawerTest
{
  private static AutoTiler GetMockedAutoTiler(uint layerCount)
  {
    AutoTileData autoTileData = new(new bool[] { true }, new());
    return new(layerCount, new AutoTileData[] { autoTileData });
  }

  private static KeyValuePair<Vector2, int>[] GetMockedPositionsToTileIds(
    Vector2[] positions, int tileId)
  {
    List<KeyValuePair<Vector2, int>> positionsToTileIds = new();
    foreach (var position in positions)
      positionsToTileIds.Add(new(position, tileId));

    return positionsToTileIds.ToArray();
  }

  [SimpleTestMethod]
  public void DrawTiles_ShouldDrawTiles_WhenMockedDrawer()
  {
    // Given
    int tileId = 0;
    Vector2[] positions = GetVector2Rectangle(Vector2.Zero, new(16, 16));

    AutoTiler autoTiler = GetMockedAutoTiler(1);
    MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTiles(0, GetMockedPositionsToTileIds(positions, tileId));

    // Then
    foreach (var position in positions)
      Assertions.AssertEqual(
        tileId, testTileMapDrawer.Data[0][new(position.X, position.Y)].TileId);
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
          tileId, testTileMapDrawer.Data[0][new(position.X, position.Y)].TileId);
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
        tileId, testTileMapDrawer.Data[0][new(position.X, position.Y)].TileId);
  }

  public static Vector2[] GetVector2Rectangle(Vector2 at, Vector2 size)
  {
    Vector2[] arr = new Vector2[(int)(size.X * size.Y)];

    int index = 0;
    for (int x = 0; x < size.X; x++)
      for (int y = 0; y < size.Y; y++)
        arr[index++] = at + new Vector2(x, y);

    return arr;
  }
}