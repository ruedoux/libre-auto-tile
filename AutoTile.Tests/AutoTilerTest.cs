using System.Text.Json;
using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;


[SimpleTestClass]
public class AutoTilerTest
{
  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    TileMaskSearcher tileMaskSearcher = new([]);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher } });

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
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher } });

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
    TileMaskSearcher tileMaskSearcher = new(items);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher } });

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
  public void PlaceTile_CorrectlyPlacesSingleTile_WhenCalled()
  {
    // Given
    string fileName = "a.jpg";

    var jsonString = File.ReadAllText("../resources/TileMasks.json");
    var mappedTileMasks = JsonSerializer.Deserialize(
      jsonString, AutoTileTestJsonContext.Default.DictionaryVector2Int32Array)
        ?? throw new ArgumentException();
    var reversedMappedTileMasks = mappedTileMasks.ToDictionary(
      kvp => TileMask.FromArray(kvp.Value), kvp => kvp.Key);
    (TileMask TileMask, TileAtlas TileAtlas)[] definedPairs = mappedTileMasks
      .Select<KeyValuePair<Vector2, int[]>, (TileMask TileMask, TileAtlas TileAtlas)>(
        kvp => new(TileMask.FromArray(kvp.Value), new TileAtlas(kvp.Key, fileName))).ToArray();

    TileMaskSearcher tileMaskSearcher = new([.. definedPairs]);
    AutoTiler autoTiler = new(1, new() { { 0, tileMaskSearcher } });
    TileDataVerifier tileDataVerifier = new(autoTiler, reversedMappedTileMasks, fileName);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
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


public class TileDataVerifier(
  AutoTiler autoTiler,
  Dictionary<TileMask, Vector2> reversedMappedTileMasks,
  string fileName)
{
  private readonly AutoTiler autoTiler = autoTiler;
  private readonly Dictionary<TileMask, Vector2> reversedMappedTileMasks = reversedMappedTileMasks;
  private readonly string fileName = fileName;

  public void PlaceTileAndVerify(int tileId, Vector2 position, TileMask tileMask)
  {
    autoTiler.PlaceTile(0, position, tileId);
    TileData tileData = autoTiler.GetTile(0, position);
    Assertions.AssertEqual(new(reversedMappedTileMasks[tileMask], fileName), tileData.TileAtlas);
  }
}