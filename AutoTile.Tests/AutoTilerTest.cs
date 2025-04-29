using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;


[SimpleTestClass]
public class AutoTilerTest
{
  const int TILE_ID = 0;

  //[SimpleTestMethod]
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

  //[SimpleTestMethod]
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

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTileMask_WhenCalled()
  {
    // Given
    string imageFileName = "a.png";
    TileAtlas TileAtlas = new(new(0, 0), imageFileName);
    TileMask target = new(0, 0, 0, -1, 2, -1, -1, -1);
    TileMaskSearcher tileMaskSearcher = new([
      new(target, TileAtlas),
      new(new(0, 0, -1, -1, 2, -1, -1, -1), new(new(0, 1), imageFileName)),
      new(new(0, 0, 0, 0, -1, 2, -1, -1), new(new(0, 2), imageFileName)),
      new(new(0, 0, 1, -1, -1, 2, -1, -1), new(new(0, 3), imageFileName))]);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher }, { 1, new([]) }, { 2, new([]) } });

    // When
    autoTiler.PlaceTile(0, Vector2.Zero, 0);
    autoTiler.PlaceTile(0, Vector2.TopLeft, 0);
    autoTiler.PlaceTile(0, Vector2.Top, 0);
    autoTiler.PlaceTile(0, Vector2.TopRight, 0);
    autoTiler.PlaceTile(0, Vector2.BottomRight, 2);

    TileData middleTileMask = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertEqual(target, middleTileMask.TileMask);
  }
}