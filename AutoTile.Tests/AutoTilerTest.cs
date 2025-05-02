using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;


[SimpleTestClass]
public class AutoTilerTest
{
  const int TILE_ID = 0;
  static readonly Vector2[] DIRECTIONS = [
    Vector2.TopLeft,
    Vector2.Top,
    Vector2.TopRight,
    Vector2.Right,
    Vector2.BottomRight,
    Vector2.Bottom,
    Vector2.BottomLeft,
    Vector2.Left];

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
    Assertions.AssertEqual(new(), tileDataAfterRemoval);
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
    var items = GetItemNoise(new(-1, 1), definedPairs);
    TileMaskSearcher tileMaskSearcher = new(items);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher }, { 1, tileMaskSearcher } });

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

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTileMask_WhenCalled()
  {
    // Given
    (TileMask TileMask, TileAtlas TileAtlas)[] definedPairs = [
      (new(1, 1, 1, 1, 1, 1, 1, 1), new(new(1, 0), "a.png")),
      (new(-1, -1, -1, -1, -1, -1, -1, 0), new(new(2, 0), "b.png")),
      (new(0, 0, 0, 0, 0, 0, 0, 0), new(new(3, 0), "c.png")),
      (new(0, -1, 0, -1, 0, -1, 0, -1), new(new(4, 0), "d.png"))];
    var items = GetItemNoise(new(-1, 1), definedPairs);

    TileMaskSearcher tileMaskSearcher = new(items);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher }, { 1, tileMaskSearcher } });

    // When
    // Then
    foreach (var definedPair in definedPairs)
      VerifyTileMaskMatches(autoTiler, 0, definedPair.TileMask, definedPair.TileAtlas);
  }

  private static void VerifyTileMaskMatches(
    AutoTiler autoTiler, int centreTileId, TileMask tileMask, TileAtlas targetTileAtlas)
  {
    var tileMaskArray = tileMask.ToArray();
    autoTiler.PlaceTile(0, Vector2.Zero, centreTileId);
    for (int i = 0; i < 8; i++)
      autoTiler.PlaceTile(0, DIRECTIONS[i], tileMaskArray[i]);

    TileData tileData = autoTiler.GetTile(0, Vector2.Zero);
    Assertions.AssertEqual(TileMask.FromArray(tileMaskArray), tileData.TileMask);
    Assertions.AssertEqual(targetTileAtlas, tileData.TileAtlas);
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