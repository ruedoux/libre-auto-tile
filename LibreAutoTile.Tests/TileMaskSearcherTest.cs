using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests;


[SimpleTestClass]
public class TileMaskSearcherTest
{
  const string IMAGE_FILE_NAME = "a";

  [SimpleTestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenExactTileMask()
  {
    // Given
    TileMask tileMask000 = new(top: 0, topRight: 0, right: 0);
    TileMask tileMask111 = new(top: 1, topRight: 1, right: 1);
    TileMask tileMask012 = new(top: 0, topRight: 1, right: 2);
    TileMask tileMask333 = new(top: 3, topRight: 3, right: 3);
    TileAtlas TileAtlas000 = new(new(0, 0), IMAGE_FILE_NAME);
    TileAtlas TileAtlas111 = new(new(0, 1), IMAGE_FILE_NAME);
    TileAtlas TileAtlas012 = new(new(0, 2), IMAGE_FILE_NAME);
    TileAtlas TileAtlas333 = new(new(0, 3), IMAGE_FILE_NAME);
    TileMaskSearcher tileMaskSearcher = new([
      new(tileMask000, TileAtlas000),
      new(tileMask111, TileAtlas111),
      new(tileMask012, TileAtlas012),
      new(tileMask333, TileAtlas333)]);

    // When
    var (resultTileMask000, resultTileAtlas000) = tileMaskSearcher.FindBestMatch(tileMask000);
    var (resultTileMask111, resultTileAtlas111) = tileMaskSearcher.FindBestMatch(tileMask111);
    var (resultTileMask012, resultTileAtlas012) = tileMaskSearcher.FindBestMatch(tileMask012);
    var (resultTileMask333, resultTileAtlas333) = tileMaskSearcher.FindBestMatch(tileMask333);

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

  [SimpleTestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenNotPreciseTileMask()
  {
    // Given
    TileAtlas TileAtlas = new(new(0, 0), "a");
    TileMask target = new(0, 0, -1, -1, -1, 2, -1, 0);
    TileMaskSearcher tileMaskSearcher = new([
      new(target, TileAtlas),
      new(new(0, 1, -1, -1, 2, -1, -1, -1), new(new(0, 1), "b")),
      new(new(0, 1, 0, 0, -1, 2, -1, -1), new(new(0, 2), "c")),
      new(new(0, 1, 1, -1, -1, 2, -1, -1), new(new(0, 3), "d"))]);

    // When
    var (resultTileMask, resultTileAtlas) = tileMaskSearcher.FindBestMatch(
      new(0, 0, -1, -1, -1, -1, -1, 0));

    // Then
    Assertions.AssertEqual(TileAtlas, resultTileAtlas);
    Assertions.AssertEqual(resultTileMask, target);
  }

  [SimpleTestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenWildCardTileMask()
  {
    // Given
    TileAtlas TileAtlas = new(new(0, 0), "a");
    TileMask target = new(0, -1, -1, -1, -1, TileMaskSearcher.WILD_CARD_ID, TileMaskSearcher.WILD_CARD_ID, TileMaskSearcher.WILD_CARD_ID);
    TileMaskSearcher tileMaskSearcher = new([
      new(target, TileAtlas),
      new(new(0, -1, -1, -1, -1, 1, 0, 0), new(new(0, 1), "b")),
      new(new(0, -1, -1, -1, -1, 0, 1, 0), new(new(0, 2), "c")),
      new(new(0, -1, -1, -1, -1, 0, 0, 1), new(new(0, 3), "d")),
      new(new(0, -1, -1, -1, -1, 0, 1, 1), new(new(0, 4), "e")),
      new(new(0, -1, -1, -1, 1, 1, 1, 1), new(new(0, 5), "f"))]);

    // When
    var (resultTileMask, resultTileAtlas) = tileMaskSearcher.FindBestMatch(
      new(0, -1, -1, -1, -1, 1, 1, 1));

    // Then
    Assertions.AssertEqual(TileAtlas, resultTileAtlas);
    Assertions.AssertEqual(resultTileMask, target);
  }

  [SimpleTestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenAllWildCardTileMask()
  {
    // Given
    TileAtlas TileAtlas = new(new(0, 0), "a");
    TileMask target = new(
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID,
      TileMaskSearcher.WILD_CARD_ID);
    TileMaskSearcher tileMaskSearcher = new([new(target, TileAtlas)]);

    // When
    var (resultTileMask, resultTileAtlas) = tileMaskSearcher.FindBestMatch(
      new(-1, 0, 1, 2, 3, 4, 5, 6));

    // Then
    Assertions.AssertEqual(TileAtlas, resultTileAtlas);
    Assertions.AssertEqual(resultTileMask, target);
  }

  [SimpleTestMethod]
  public void FindBestMatch_ShouldReturnFirstMask_WhenNoMatches()
  {
    // Given
    TileAtlas tileAtlas1 = new(new(0, 0), IMAGE_FILE_NAME);
    TileAtlas tileAtlas2 = new(new(0, 1), IMAGE_FILE_NAME);
    TileMask target1 = new(1, 2, 3, 4, 5, 6, 7, 8);
    TileMask target2 = new(2, 3, 4, 5, 6, 7, 8, 9);
    TileMaskSearcher tileMaskSearcher = new([new(target1, tileAtlas1), new(target2, tileAtlas2)]);

    // When
    var (resultTileMask, resultTileAtlas) = tileMaskSearcher.FindBestMatch(new(0, 0, 0));

    // Then
    Assertions.AssertEqual(tileAtlas1, resultTileAtlas);
    Assertions.AssertEqual(target1, resultTileMask);
  }

  [SimpleTestMethod]
  public void Lookup_ShouldFindBestTileMask_WhenMatchingPatternNoiseHigh()
  {
    // Given
    TileAtlas tileAtlas = new(new(0, 0), IMAGE_FILE_NAME);
    TileMask target = new(0, 0, -1, -1, -1, -1, -1, 0);
    Random random = new(8008);
    List<(TileMask, TileAtlas)> items = Enumerable.Range(0, 1000)
      .Select(_ => (
        new TileMask(
          topLeft: 0,
          top: 0,
          topRight: random.Next(-100, 100),
          right: random.Next(-100, 100),
          bottomRight: random.Next(-100, 100),
          bottom: random.Next(-100, 100),
          bottomLeft: random.Next(-100, 100),
          left: -1
        ),
        new TileAtlas(new(0, 1), IMAGE_FILE_NAME)
      )).ToList();

    items.Add(new(target, tileAtlas));
    TileMaskSearcher tileMaskSearcher = new(items);

    // When
    var (resultTileMask, resultTileAtlas) = tileMaskSearcher.FindBestMatch(
      new(0, 0, -1, -1, -1, -1, -1, 0));

    // Then
    Assertions.AssertEqual(tileAtlas, resultTileAtlas);
    Assertions.AssertEqual(target, resultTileMask);
  }
}