using System.Text.Json;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests;


[SimpleTestClass]
public class AutoTilerTest
{
  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    TileMaskSearcher tileMaskSearcher = new([(new(), new())]);
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
    TileMaskSearcher tileMaskSearcher = new([(new(), new())]);
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
  public void PlaceTile_CorrectlyPlacesSingleTileTransientFilledSquare_WhenCalled()
  {
    // Given
    var jsonString = File.ReadAllText("resources/AutoTileConfigurationTransient.json");
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTilerComposer autoTilerComposer = new(autoTileConfiguration, false);
    AutoTiler autoTiler = autoTilerComposer.GetAutoTiler(1);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(0, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, -1, -1, -1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(0, 0, 0, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, 0, 0, 0, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, -1, -1, -1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(0, 0, -1, -1, -1, -1, -1, 0));
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesSingleTileTransientEmptySquare_WhenCalled()
  {
    // Given
    var jsonString = File.ReadAllText("resources/AutoTileConfigurationTransient.json");
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTilerComposer autoTilerComposer = new(autoTileConfiguration, false);
    AutoTiler autoTiler = autoTilerComposer.GetAutoTiler(1);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));

  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleTileTransientFilledSquare_WhenCalled()
  {
    // Given
    var jsonString = File.ReadAllText("resources/AutoTileConfigurationTransient.json");
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTilerComposer autoTilerComposer = new(autoTileConfiguration, false);
    AutoTiler autoTiler = autoTilerComposer.GetAutoTiler(1);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 1);

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Zero, new(1, 1, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(1, 0, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(1, 0, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(1, 1, 1, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(1, 0, 1, 1, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(1, 1, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(1, 0, 1, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 1, 1, 0, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 1, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(1, 1, 1, 1, 1, 1, 1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(0, 0, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(0, 0, 0, 0, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(0, 0, 1, 1, 1, 1, 1, 0));
  }

  //[SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleTileTransientEmptySquare_WhenCalled()
  {
    // Given
    var jsonString = File.ReadAllText("resources/AutoTileConfigurationTransient.json");
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTilerComposer autoTilerComposer = new(autoTileConfiguration, false);
    AutoTiler autoTiler = autoTilerComposer.GetAutoTiler(1);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 1);

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(1, 1, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(1, Vector2.Zero, new(1, 1, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(1, 1, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(1, Vector2.Zero, new(1, 0, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(1, 1, 1, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(1, Vector2.Zero, new(1, 0, 1, 1, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(1, 1, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(1, Vector2.Zero, new(1, 0, 1, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 1, 1, 0, 1, 1, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(1, Vector2.Zero, new(0, 0, 1, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(1, 1, 1, 1, 1, 1, 1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(1, Vector2.Zero, new(0, 0, 0, 0, 1, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 1, 1, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(0, 0, 1, 1, 1, 1, 1, 0));
    tileDataVerifier.Verify(1, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(1, 0, 0, 0, 1, 1, 1, 1));
    tileDataVerifier.Verify(1, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(1, 1, 1, 0, 0, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(0, 0, 0, 0, 1, 1, 1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(1, 0, 0, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.Right, new(0, 0, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(1, 1, 1, 0, 0, 0, 1, 1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(1, 1, 1, 1, 1, 0, 0, 0));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(0, 0, 1, 1, 1, 1, 1, 0));
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


public class TileDataVerifier(AutoTiler autoTiler, AutoTileConfiguration autoTileConfiguration)
{
  private readonly AutoTiler autoTiler = autoTiler;
  private readonly AutoTileConfiguration autoTileConfiguration = autoTileConfiguration;

  public void PlaceTileAndVerify(int tileId, Vector2 position, TileMask tileMask)
  {
    autoTiler.PlaceTile(0, position, tileId);
    Verify(tileId, position, tileMask);
  }

  public void Verify(int tileId, Vector2 position, TileMask tileMask)
  {
    TileData resultTileData = autoTiler.GetTile(0, position);
    var shouldBeMaskAndAtlas = GetAtlasAndMaskFromConfiguration(tileId, tileMask);
    Assertions.AssertEqual(shouldBeMaskAndAtlas.TileAtlas, resultTileData.TileAtlas,
      $"Mask is {resultTileData.TileMask}, but should be: {shouldBeMaskAndAtlas.TileMask}");
  }

  private (TileAtlas TileAtlas, TileMask TileMask) GetAtlasAndMaskFromConfiguration(
    int tileId, TileMask tileMask)
  {
    var tileDefinition = autoTileConfiguration.TileDefinitions[(uint)tileId];
    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (atlasPosition, tileMasks) in tileMaskDefinition.AtlasPositionToTileMasks)
      {
        foreach (var tileMaskArray in tileMasks)
        {
          var candidateTileMask = TileMask.FromArray([.. tileMaskArray]);
          if (candidateTileMask == tileMask)
            return (new(atlasPosition.ToVector2(), imageFileName), candidateTileMask);
        }
      }
    }

    throw new ArgumentException($"Unable to find mask for tile id '{tileId}' and tile mask: {tileMask}");
  }
}