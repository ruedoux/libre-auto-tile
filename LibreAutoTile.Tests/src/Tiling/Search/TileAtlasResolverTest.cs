using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling.Search;

[TestClass]
public class TileAtlasResolverTest
{
  [TestMethod]
  public void GetTileAtlas_ShouldReturnAtlas_WhenGivenAtlasExists()
  {
    // Given
    TileAtlas targetAtlas = new(new(0, 0), "a");
    TileMask targetMask = new(topLeft: 0, top: 0);
    TileAtlasResolver tileAtlasResolver = new([
      new(targetMask, targetAtlas),
      new(new(topLeft: 0, topRight:1), new(new(0, 1), "b")),
      new(new(topLeft: 0, topRight:2), new(new(0, 2), "c")),
      new(new(topLeft: 0, topRight:3), new(new(0, 3), "d")),
      new(new(topLeft: 0, topRight:4), new(new(0, 4), "e"))]);

    // When
    var resultTileAtlas = tileAtlasResolver.GetTileAtlas(targetMask);

    // Then
    Assertions.AssertEqual(targetAtlas, resultTileAtlas);
  }

  [TestMethod]
  public void GetTileAtlas_ShouldReturnRandomAtlas_WhenMultipleAtlasesExists()
  {
    // Given
    TileMask tileMask1 = new(topLeft: 0, topRight: 0);
    TileMask tileMask2 = new(topLeft: 1, topRight: 1);
    TileAtlas[] tileAtlases1 = [new(new(0, 1), "b", int.MaxValue / 2), new(new(0, 2), "c", int.MaxValue)];
    TileAtlas[] tileAtlases2 = [new(new(0, 3), "d", int.MaxValue / 2), new(new(0, 4), "e", int.MaxValue)];

    TileAtlasResolver tileAtlasResolver = new([
      new(tileMask1, tileAtlases1[0]),
      new(tileMask1, tileAtlases1[1]),
      new(tileMask2, tileAtlases2[0]),
      new(tileMask2, tileAtlases2[1])]);

    // When
    var resultTileAtlas1 = tileAtlasResolver.GetTileAtlas(tileMask1);
    var resultTileAtlas2 = tileAtlasResolver.GetTileAtlas(tileMask2);

    // Then
    Assertions.AssertTrue(tileAtlases1.Contains(resultTileAtlas1));
    Assertions.AssertTrue(tileAtlases2.Contains(resultTileAtlas2));
  }

  [TestMethod]
  public void GetTileAtlas_ShouldReturnDefaultAtlas_WhenGivenDoesNotExist()
  {
    // Given
    TileAtlasResolver tileAtlasResolver = new([
      new(new(topLeft: 0, topRight:1), new(new(0, 1), "b")),
      new(new(topLeft: 0, topRight:2), new(new(0, 2), "c")),
      new(new(topLeft: 0, topRight:3), new(new(0, 3), "d")),
      new(new(topLeft: 0, topRight:4), new(new(0, 4), "e"))]);

    // When
    var resultTileAtlas = tileAtlasResolver.GetTileAtlas(new(topLeft: 9, topRight: 9));

    // Then
    Assertions.AssertEqual(new TileAtlas(), resultTileAtlas);
  }
}