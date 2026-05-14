using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling.Search;

[SimpleTestClass]
public class TileSearcherTest
{
  [SimpleTestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenExactTileMask()
  {
    // Given
    TileMask tileMask000 = new(top: 0, topRight: 0, right: 0);
    TileMask tileMask111 = new(top: 1, topRight: 1, right: 1);
    TileMask tileMask012 = new(top: 0, topRight: 1, right: 2);
    TileMask tileMask333 = new(top: 3, topRight: 3, right: 3);
    TileAtlas TileAtlas000 = new(new(0, 0), "a");
    TileAtlas TileAtlas111 = new(new(0, 1), "b");
    TileAtlas TileAtlas012 = new(new(0, 2), "c");
    TileAtlas TileAtlas333 = new(new(0, 3), "d");

    (TileMask TileMask, TileAtlas TileAtlas)[] items = [
      new(tileMask000, TileAtlas000),
      new(tileMask111, TileAtlas111),
      new(tileMask012, TileAtlas012),
      new(tileMask333, TileAtlas333)];
    TileMaskSearcher tileMaskSearcher = new(items.Select(x => x.TileMask));
    TileAtlasResolver tileAtlasResolver = new(items);
    TileSearcher tileSearcher = new(tileMaskSearcher, tileAtlasResolver);

    // When
    var (resultTileMask000, resultTileAtlas000) = tileSearcher.FindBestMatch(tileMask000);
    var (resultTileMask111, resultTileAtlas111) = tileSearcher.FindBestMatch(tileMask111);
    var (resultTileMask012, resultTileAtlas012) = tileSearcher.FindBestMatch(tileMask012);
    var (resultTileMask333, resultTileAtlas333) = tileSearcher.FindBestMatch(tileMask333);

    // Then
    Assertions.AssertEqual(TileAtlas000, resultTileAtlas000);
    Assertions.AssertEqual(TileAtlas111, resultTileAtlas111);
    Assertions.AssertEqual(TileAtlas012, resultTileAtlas012);
    Assertions.AssertEqual(TileAtlas333, resultTileAtlas333);

    Assertions.AssertEqual(tileMask000, resultTileMask000);
    Assertions.AssertEqual(tileMask111, resultTileMask111);
    Assertions.AssertEqual(tileMask012, resultTileMask012);
    Assertions.AssertEqual(tileMask333, resultTileMask333);
  }
}