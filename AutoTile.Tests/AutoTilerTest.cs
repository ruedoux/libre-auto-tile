using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;


[SimpleTestClass]
public class AutoTilerTest
{
  const int TILE_ID = 0;

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    TileMaskSearcher tileMaskSearcher = new([]);
    AutoTiler autoTiler = new(1, new() { { TILE_ID, tileMaskSearcher } });

    // When
    autoTiler.PlaceTile(0, Vector2.Zero, 0);
    TileData tileData = autoTiler.GetTile(0, Vector2.Zero);
    autoTiler.PlaceTile(0, Vector2.Zero, -1);
    var tileDataAfterRemoval = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertEqual(default, tileDataAfterRemoval);
    Assertions.AssertNotNull(tileData);
    Assertions.AssertEqual(0, tileData.CentreTileId);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTiles_WhenCalledAsync()
  {
    // Given
    TileMaskSearcher tileMaskSearcher = new([]);
    AutoTiler autoTiler = new(1, new() { { TILE_ID, tileMaskSearcher } });

    List<Vector2> positions = [];
    for (int x = 0; x < 10; x++)
      for (int y = 0; y < 10; y++)
        positions.Add(new(x, y));

    // When
    List<Task> tasks = [];
    for (int i = 0; i < 10; i++)
    {
      tasks.Add(new Task(() =>
      {
        foreach (var position in positions)
          autoTiler.PlaceTile(0, position, 0);
      }));
    }

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
}