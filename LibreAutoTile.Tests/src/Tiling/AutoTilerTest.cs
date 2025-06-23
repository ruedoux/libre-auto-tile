using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;


[SimpleTestClass]
public class AutoTilerTest
{
  private string jsonString = "";

  [SimpleBeforeAll]
  public void BeforeAll()
  {
    jsonString = File.ReadAllText("../resources/configurations/ExampleConfigurationTransient.json");
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new([]) } });

    // When
    autoTiler.PlaceTile(0, Vector2.Zero, 0);
    TileData tileData = autoTiler.GetTile(0, Vector2.Zero);
    autoTiler.PlaceTile(0, Vector2.Zero, -1);
    var tileDataAfterRemoval = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertEqual(new(), tileDataAfterRemoval);
    Assertions.AssertNotNull(tileData);
    Assertions.AssertEqual(0, tileData.CentreTileId);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTiles_WhenCalledAsync()
  {
    // Given
    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new([]) } });

    List<Vector2> positions = [];
    for (int x = 0; x < 10; x++)
      for (int y = 0; y < 10; y++)
        positions.Add(new(x, y));

    // When
    List<Task> tasks = [];
    for (int i = 0; i < 10; i++)
      foreach (var position in positions)
        tasks.Add(new Task(() => autoTiler.PlaceTile(0, position, 0)));

    foreach (var task in tasks)
      task.Start();

    Task.WhenAll(tasks).Wait();

    // Then
    foreach (var position in positions)
    {
      var tileData = autoTiler.GetTile(0, position);
      Assertions.AssertNotNull(tileData);
      Assertions.AssertEqual(0, tileData.CentreTileId);
    }
  }

  [SimpleTestMethod]
  public void PlaceTile_PlacesAndRemovesTile_WhenCalled()
  {
    // Given
    (TileMask TileMask, TileAtlas TileAtlas)[] definedPairs = [
      (new(-1, -1, -1, -1, -1, -1, -1, -1), new(new(1, 0), "a.png"))];
    var items = GetItemNoise(new(-1, 0), definedPairs);
    AutoTiler autoTiler = new(1, new Dictionary<int, TileMaskSearcher>() { { 0, new(items) } });

    // When
    autoTiler.PlaceTile(0, Vector2.Zero, 0);
    var beforeRemoval = autoTiler.GetTile(0, Vector2.Zero);

    autoTiler.PlaceTile(0, Vector2.Zero, -1);
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