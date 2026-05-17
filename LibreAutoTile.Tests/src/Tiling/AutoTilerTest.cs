using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;


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
  [TestMethodArguments(0, 1)]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled(int mapSize)
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } }, new(mapSize, mapSize));

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
  [TestMethodArguments(0, 10)]
  public void PlaceTile_CorrectlyPlacesTiles_WhenCalledAsync(int mapSize)
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new([], []), new([])) } }, new(mapSize, mapSize));

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
  [TestMethodArguments(0, 1)]
  public void PlaceTile_PlacesAndRemovesTile_WhenCalled(int mapSize)
  {
    // Given
    (TileMask TileMask, TileAtlas TileAtlas)[] definedPairs = [
      (new(-1, -1, -1, -1, -1, -1, -1, -1), new(new(1, 0), "a.png"))];
    var items = GetItemNoise(new(-1, 0), definedPairs);

    AutoTiler autoTiler = new(1, new Dictionary<int, TileSearcher>() { { 0, new(new(definedPairs.Select(x => x.TileMask), []), new(definedPairs)) } }, new(mapSize, mapSize));

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