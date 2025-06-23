using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.SimpleTest;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using System.Security.Cryptography;
using Qwaitumin.LibreAutoTile.Tiling.Search;

namespace Qwaitumin.LibreAutoTile.Tests;


[SimpleTestClass]
public class AutoTileDrawerTest
{
  [SimpleTestMethod]
  public void DrawTiles_ShouldDrawTiles_WhenMockedDrawer()
  {
    // Given
    Vector2[] positions = Helpers.GetVector2Rectangle(Vector2.Zero, new(16, 16));
    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new([]) } }); MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTiles(0, Helpers.GetMockedPositionsToTileIds(positions));

    // Then
    foreach (var position in positions)
      Assertions.AssertEqual(
        Helpers.TILE_ID, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
  }

  [SimpleTestMethod]
  public void DrawTilesAsync_ShouldDrawTiles_WhenMockedDrawer()
  {
    // Given
    Vector2[] positions = Helpers.GetVector2Rectangle(Vector2.Zero, new(16, 16));

    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new([]) } });
    MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTilesAsync(0, Helpers.GetMockedPositionsToTileIds(positions));

    // Then
    Assertions.AssertAwaitAtMost(100, () => // 100ms is more than enough for 16x16 tiles
    {
      foreach (var position in positions)
        Assertions.AssertEqual(
          Helpers.TILE_ID, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
    });
  }

  [SimpleTestMethod]
  public void DrawTilesAsync_ShouldDrawTiles_WhenMockedDrawerAndWait()
  {
    // Given
    Vector2[] positions = Helpers.GetVector2Rectangle(Vector2.Zero, new(16, 16));

    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new([]) } }); MockedTileMapDrawer testTileMapDrawer = new(1);
    AutoTileDrawer autoTileDrawer = new(testTileMapDrawer, autoTiler);

    // When
    autoTileDrawer.DrawTilesAsync(0, Helpers.GetMockedPositionsToTileIds(positions));
    autoTileDrawer.Wait();

    // Then
    foreach (var position in positions)
      Assertions.AssertEqual(
        Helpers.TILE_ID, testTileMapDrawer.Data[0][new(position.X, position.Y)].CentreTileId);
  }
}


