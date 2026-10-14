using LibreAutoTile.Configuration.Models;
using LibreAutoTile.Tiling;
using LibreAutoTile.Tiling.Search;
using LibreAutoTile.Tiling.Search.Models;
using SimpleTest;

namespace LibreAutoTile.Tests.Tiling;


[TestClass]
public class AutoTilerTest
{
  [TestMethod]
  public void PlaceTile_ThrowsException_WhenWrongLayer()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } });

    // When
    // Then
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => autoTiler.PlaceTiles(1, [(new(), 0)]));
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => autoTiler.PlaceTiles(-1, [(new(), 0)]));
  }

  [TestMethod]
  public void PlaceTile_ThrowsException_WhenWrongTileId()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } });

    // When
    // Then
    Assertions.AssertThrows<ArgumentException>(
      () => autoTiler.PlaceTiles(0, [(new(), 1)]));
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } });

    // When
    autoTiler.PlaceTiles(0, [(Vector2.Zero, 0)]);
    var tileData = autoTiler.GetTile(0, Vector2.Zero);
    autoTiler.PlaceTiles(0, [(Vector2.Zero, -1)]);
    var tileDataAfterRemoval = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertEqual(new(), tileDataAfterRemoval);
    Assertions.AssertNotNull(tileData);
    Assertions.AssertEqual(0, tileData.CentreTileId);
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesTiles_WhenCalledAsync()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } });

    List<(Vector2, int)> tiles = [];
    for (int x = 0; x < 10; x++)
      for (int y = 0; y < 10; y++)
        tiles.Add(new(new(x, y), 0));

    // When
    List<Task> tasks = [];
    for (int i = 0; i < 10; i++)
      tasks.Add(new Task(() => autoTiler.PlaceTiles(0, tiles)));

    foreach (var task in tasks)
      task.Start();

    Task.WhenAll(tasks).Wait();

    // Then
    foreach (var tile in tiles)
    {
      var tileData = autoTiler.GetTile(0, tile.Item1);
      Assertions.AssertEqual(0, tileData.CentreTileId);
    }
  }

  [TestMethod]
  public void PlaceTile_PlacesAndRemovesTile_WhenCalled()
  {
    // Given
    (TileMask TileMask, TileAtlas TileAtlas)[] definedPairs = [
      (new(-1, -1, -1, -1, -1, -1, -1, -1), new(new(1, 0), "a.png"))];
    var items = GetItemNoise(new(-1, 0), definedPairs);

    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new(definedPairs.Select(x => x.TileMask), []), new(definedPairs)) } });

    // When
    autoTiler.PlaceTiles(0, [(Vector2.Zero, 0)]);
    var beforeRemoval = autoTiler.GetTile(0, Vector2.Zero);

    autoTiler.PlaceTiles(0, [(Vector2.Zero, -1)]);
    var afterRemoval = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertEqual(0, beforeRemoval.CentreTileId);
    Assertions.AssertEqual(definedPairs[0].TileMask, beforeRemoval.TileMask);
    Assertions.AssertEqual(definedPairs[0].TileAtlas, beforeRemoval.TileAtlas);
    Assertions.AssertEqual(-1, afterRemoval.CentreTileId);
    Assertions.AssertEqual(new(), afterRemoval.TileMask);
    Assertions.AssertEqual(new(), afterRemoval.TileAtlas);
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenNegativePosition()
  {
    // Given
    AutoTiler autoTiler = new(1, BuildSingleTileSearchers(), chunkSize: 64);
    var position = new Vector2(-1, -1);

    // When
    autoTiler.PlaceTiles(0, [(position, 0)]);
    var tileData = autoTiler.GetTile(0, position);

    // Then
    Assertions.AssertEqual(0, tileData.CentreTileId);
  }

  [TestMethod]
  public void PlaceTile_CorrectlyRecomputesTiles_WhenCrossingChunkBoundary()
  {
    // Given
    AutoTiler autoTiler = new(1, BuildSingleTileSearchers(), chunkSize: 64);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(63, 0), 0), (new Vector2(64, 0), 0)]);

    // Then
    Assertions.AssertEqual(0, autoTiler.GetTile(0, new Vector2(63, 0)).CentreTileId);
    Assertions.AssertEqual(0, autoTiler.GetTile(0, new Vector2(64, 0)).CentreTileId);
  }

  [TestMethod]
  public void PlaceTile_RecomputesTiles_WhenCrossingHorizontalChunkBoundary_WithDefaultChunkSize()
  {
    // Given
    AutoTiler autoTiler = BuildAutoTilerWithMasks([
      new(-1, -1, -1, 0, -1, -1, -1, -1),   // Right neighbour connected
      new(-1, -1, -1, -1, -1, -1, -1, 0),   // Left neighbour connected
    ]);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(127, 0), 0), (new Vector2(128, 0), 0)]);

    // Then
    Assertions.AssertEqual(
      new(-1, -1, -1, 0, -1, -1, -1, -1), autoTiler.GetTile(0, new Vector2(127, 0)).TileMask);
    Assertions.AssertEqual(
      new(-1, -1, -1, -1, -1, -1, -1, 0), autoTiler.GetTile(0, new Vector2(128, 0)).TileMask);
  }

  [TestMethod]
  public void PlaceTile_RecomputesTiles_WhenCrossingVerticalChunkBoundary_WithDefaultChunkSize()
  {
    // Given
    AutoTiler autoTiler = BuildAutoTilerWithMasks([
      new(-1, -1, -1, -1, -1, 0, -1, -1),   // Bottom neighbour connected
      new(-1, 0, -1, -1, -1, -1, -1, -1),   // Top neighbour connected
    ]);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(0, 127), 0), (new Vector2(0, 128), 0)]);

    // Then
    Assertions.AssertEqual(
      new(-1, -1, -1, -1, -1, 0, -1, -1), autoTiler.GetTile(0, new Vector2(0, 127)).TileMask);
    Assertions.AssertEqual(
      new(-1, 0, -1, -1, -1, -1, -1, -1), autoTiler.GetTile(0, new Vector2(0, 128)).TileMask);
  }

  [TestMethod]
  public void PlaceTile_RecomputesTiles_WhenCrossingNegativeChunkBoundary()
  {
    // Given
    AutoTiler autoTiler = BuildAutoTilerWithMasks([
      new(-1, -1, -1, 0, -1, -1, -1, -1),   // Right neighbour connected
      new(-1, -1, -1, -1, -1, -1, -1, 0),   // Left neighbour connected
    ]);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(-1, 0), 0), (new Vector2(0, 0), 0)]);

    // Then
    Assertions.AssertEqual(
      new(-1, -1, -1, 0, -1, -1, -1, -1), autoTiler.GetTile(0, new Vector2(-1, 0)).TileMask);
    Assertions.AssertEqual(
      new(-1, -1, -1, -1, -1, -1, -1, 0), autoTiler.GetTile(0, new Vector2(0, 0)).TileMask);
  }

  [TestMethod]
  public void GetTile_ReturnsEmpty_WhenReadingUntouchedCellInFreshChunk()
  {
    // Given
    AutoTiler autoTiler = new(1, BuildSingleTileSearchers(), chunkSize: 64);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(0, 0), 0)]);
    var untouchedCell = autoTiler.GetTile(0, new Vector2(10, 10));

    // Then
    Assertions.AssertTrue(untouchedCell.IsEmpty());
  }

  [TestMethod]
  public void PlaceTile_ReusesChunk_WhenRemovedThenPlacedAgain()
  {
    // Given
    AutoTiler autoTiler = new(1, BuildSingleTileSearchers(), chunkSize: 64);
    var position = new Vector2(5, 5);

    // When
    autoTiler.PlaceTiles(0, [(position, 0)]);
    Assertions.AssertEqual(0, autoTiler.GetTile(0, position).CentreTileId);

    autoTiler.PlaceTiles(0, [(position, -1)]);
    Assertions.AssertTrue(autoTiler.GetTile(0, position).IsEmpty());

    autoTiler.PlaceTiles(0, [(position, 0)]);

    // Then
    Assertions.AssertEqual(0, autoTiler.GetTile(0, position).CentreTileId);
  }

  [TestMethod]
  public void Constructor_ThrowsException_WhenChunkSizeNotPowerOfTwo()
  {
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => new AutoTiler(1, BuildSingleTileSearchers(), chunkSize: 65));
  }

  [TestMethod]
  public void Constructor_ThrowsException_WhenNegativeChunkPoolCapacity()
  {
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => new AutoTiler(1, BuildSingleTileSearchers(), chunkSize: 64, chunkPoolCapacity: -1));
  }

  [TestMethod]
  public void Clear_ReusesChunk_WithoutResurrectingStaleTiles()
  {
    // Given
    AutoTiler autoTiler = new(1, BuildSingleTileSearchers(), chunkSize: 64);

    // When
    autoTiler.PlaceTiles(0, [(new Vector2(0, 0), 0)]);
    autoTiler.Clear();

    // Then
    Assertions.AssertTrue(autoTiler.GetTile(0, new Vector2(0, 0)).IsEmpty());

    // When (replant into a chunk recycled by Clear)
    autoTiler.PlaceTiles(0, [(new Vector2(5, 5), 0)]);

    // Then
    Assertions.AssertEqual(0, autoTiler.GetTile(0, new Vector2(5, 5)).CentreTileId);
    Assertions.AssertTrue(autoTiler.GetTile(0, new Vector2(0, 0)).IsEmpty());
  }

  private static Dictionary<int, TileSearcher> BuildSingleTileSearchers()
    => new() { { 0, new(new([], []), new([])) } };

  private static AutoTiler BuildAutoTilerWithMasks(TileMask[] masks, int chunkSize = 128)
  {
    (TileMask, TileAtlas)[] definedPairs = masks
      .Select(mask => (mask, new TileAtlas(new(1, 0), "a.png")))
      .ToArray();

    return new AutoTiler(
      1,
      new Dictionary<int, TileSearcher>()
      {
        { 0, new(new(definedPairs.Select(pair => pair.Item1), []), new(definedPairs)) }
      },
      chunkSize: chunkSize);
  }

  private static List<(TileMask, TileAtlas)> GetItemNoise(
    Vector2 range, (TileMask, TileAtlas)[] definedPairs, int count = 10)
  {
    Random random = new();
    var items = Enumerable.Range(0, count)
      .Select(_ => (
        new TileMask(
          topLeft: random.Next(range.X, range.Y),
          top: random.Next(range.X, range.Y),
          topRight: random.Next(range.X, range.Y),
          right: random.Next(range.X, range.Y),
          bottomRight: random.Next(range.X, range.Y),
          bottom: random.Next(range.X, range.Y),
          bottomLeft: random.Next(range.X, range.Y),
          left: random.Next(range.X, range.Y)
        ),
        new TileAtlas()
      )).ToList();

    foreach (var definedPair in definedPairs)
    {
      items.RemoveAll(item => item.Item1 == definedPair.Item1);
      items.Add(definedPair);
    }

    return items;
  }
}