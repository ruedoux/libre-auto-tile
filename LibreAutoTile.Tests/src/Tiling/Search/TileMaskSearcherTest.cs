using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling.Search;


[TestClass]
public class TileMaskSearcherTest
{
  [TestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenExactTileMask()
  {
    // Given
    TileMask tileMask000 = new(top: 0, topRight: 0, right: 0);
    TileMask tileMask111 = new(top: 1, topRight: 1, right: 1);
    TileMask tileMask012 = new(top: 0, topRight: 1, right: 2);
    TileMask tileMask333 = new(top: 3, topRight: 3, right: 3);
    TileMaskSearcher tileMaskSearcher = new([
        tileMask000 ,tileMask111 ,tileMask012 ,tileMask333 ]);

    // When
    var resultTileMask000 = tileMaskSearcher.FindBestMatch(tileMask000);
    var resultTileMask111 = tileMaskSearcher.FindBestMatch(tileMask111);
    var resultTileMask012 = tileMaskSearcher.FindBestMatch(tileMask012);
    var resultTileMask333 = tileMaskSearcher.FindBestMatch(tileMask333);

    // Then
    Assertions.AssertEqual(tileMask000, resultTileMask000);
    Assertions.AssertEqual(tileMask111, resultTileMask111);
    Assertions.AssertEqual(tileMask012, resultTileMask012);
    Assertions.AssertEqual(tileMask333, resultTileMask333);
  }

  [TestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenNotPreciseTileMask()
  {
    // Given
    TileMask target = new(topLeft: 0, top: 0);
    TileMaskSearcher tileMaskSearcher = new([
      target,
      new(topLeft: 0, top: 1),
      new(topLeft: 0, top: 2),
      new(topLeft: 0, top: 3)]);

    // When
    var resultTileMask = tileMaskSearcher.FindBestMatch(new(topLeft: 0, top: 0, topRight: 0));

    // Then
    Assertions.AssertEqual(resultTileMask, target);
  }

  [TestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenWildCardTileMask()
  {
    // Given
    TileMask target = new(topLeft: 0, top: TileMaskSearcher.DEFAULT_WILDCARD_ID);
    TileMaskSearcher tileMaskSearcher = new([
      target,
      new(topLeft: 0, topRight:1),
      new(topLeft: 0, topRight:2),
      new(topLeft: 0, topRight:3),
      new(topLeft: 0, topRight:4)]);

    // When
    var resultTileMask = tileMaskSearcher.FindBestMatch(
      new(topLeft: 0, top: 999));

    // Then
    Assertions.AssertEqual(resultTileMask, target);
  }

  [TestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenAllWildCardTileMask()
  {
    // Given
    TileMask target = new(
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID,
      TileMaskSearcher.DEFAULT_WILDCARD_ID);
    TileMaskSearcher tileMaskSearcher = new([target]);

    // When
    var resultTileMask = tileMaskSearcher.FindBestMatch(
      new(-1, 0, 1, 2, 3, 4, 5, 6));

    // Then
    Assertions.AssertEqual(resultTileMask, target);
  }

  [TestMethod]
  public void FindBestMatch_ShouldReturnFirstMask_WhenNoMatches()
  {
    // Given
    TileMask target1 = new(1, 2, 3, 4, 5, 6, 7, 8);
    TileMask target2 = new(2, 3, 4, 5, 6, 7, 8, 9);
    TileMaskSearcher tileMaskSearcher = new([target1, target2]);

    // When
    var resultTileMask = tileMaskSearcher.FindBestMatch(new(0, 0, 0));

    // Then
    Assertions.AssertEqual(target1, resultTileMask);
  }

  [TestMethod]
  public void Lookup_ShouldFindBestTileMask_WhenMatchingPatternNoiseHigh()
  {
    // Given
    TileAtlas tileAtlas = new(new(0, 0), "a");
    TileMask target = new(topLeft: 0, top: 0);
    Random random = new(8008);
    List<TileMask> items = [.. Enumerable.Range(0, 1000)
      .Select(_ => (
        new TileMask(
          topLeft: 0,
          top: -1,
          topRight: random.Next(-100, 100),
          right: random.Next(-100, 100),
          bottomRight: random.Next(-100, 100),
          bottom: random.Next(-100, 100),
          bottomLeft: random.Next(-100, 100),
          left: random.Next(-100, 100)
        )
      ))];

    items.Add(target);
    TileMaskSearcher tileMaskSearcher = new(items, []);

    // When
    var resultTileMask = tileMaskSearcher.FindBestMatch(new(top: 0));

    // Then
    Assertions.AssertEqual(target, resultTileMask);
  }

  [TestMethod]
  public void FindBestMatch_ShouldFindBestResult_WhenGivenConnectionGroupIds()
  {
    // Given
    int connectionGroupId1 = 0;
    int connectionGroupId2 = 1;
    TileMask target = new(top: connectionGroupId1, right: connectionGroupId2);
    TileMaskSearcher tileMaskSearcher = new([
      target,
      new(top:connectionGroupId1, right:-1),
      new(top:-1, right:connectionGroupId1),
      new(top:connectionGroupId2, right:-1),
      new(top:-1, right:connectionGroupId2)],
      connectionGroupTileIds: [connectionGroupId1, connectionGroupId2]);

    // When
    var resultTileMask1 = tileMaskSearcher.FindBestMatch(
      new(top: connectionGroupId1, right: connectionGroupId2));
    var resultTileMask2 = tileMaskSearcher.FindBestMatch(
      new(top: connectionGroupId1, right: connectionGroupId1));
    var resultTileMask3 = tileMaskSearcher.FindBestMatch(
      new(top: connectionGroupId2, right: connectionGroupId2));
    var resultTileMask4 = tileMaskSearcher.FindBestMatch(
      new(top: connectionGroupId2, right: connectionGroupId1));

    // Then
    Assertions.AssertEqual(resultTileMask1, target);
    Assertions.AssertEqual(resultTileMask2, target);
    Assertions.AssertEqual(resultTileMask3, target);
    Assertions.AssertEqual(resultTileMask4, target);
  }

  [TestMethod]
  public void FindBestMatch_ShouldMatchSerialResults_WhenCalledConcurrently()
  {
    // Given
    Random random = new(123);
    TileMask[] items = [.. Enumerable.Range(0, 512)
      .Select(i => new TileMask(
        topLeft: random.Next(-1,1000),
        top: random.Next(-1,1000),
        topRight: random.Next(-1,1000),
        right: random.Next(-1,1000),
        bottomRight: random.Next(-1,1000),
        bottom: random.Next(-1,1000),
        bottomLeft: random.Next(-1,1000),
        left: random.Next(-1,1000)))];

    TileMask[] targets = [.. Enumerable.Range(0, 2048)
      .Select(i => new TileMask(
        topLeft: random.Next(-1,1000),
        top: random.Next(-1,1000),
        topRight: random.Next(-1,1000),
        right: random.Next(-1,1000),
        bottomRight: random.Next(-1,1000),
        bottom: random.Next(-1,1000),
        bottomLeft: random.Next(-1,1000),
        left: random.Next(-1,1000)))];

    TileMaskSearcher serialSearcher = new(items);
    TileMask[] expectedResults = [.. targets.Select(serialSearcher.FindBestMatch)];
    TileMaskSearcher parallelSearcher = new(items);
    TileMask[] actualResults = new TileMask[targets.Length];

    // When
    Parallel.For(0, targets.Length, i =>
      actualResults[i] = parallelSearcher.FindBestMatch(targets[i]));

    // Then
    for (int i = 0; i < targets.Length; i++)
      Assertions.AssertEqual(expectedResults[i], actualResults[i]);
  }
}