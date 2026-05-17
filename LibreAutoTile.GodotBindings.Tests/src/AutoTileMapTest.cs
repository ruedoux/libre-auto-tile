using Qwaitumin.SimpleTest;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.Configuration;
using Godot;

namespace LibreAutoTile.GodotBindings.Tests;

[TestClass]
public class AutoTileMapTest
{
  private enum TILES { GRASS = 0, WATER = 1 }
  private const string CONFIG_PATH = "../resources/configurations/ExampleConfigurationTransient.json";


  [TestMethod]
  public void DrawTiles_PlacesTileMapTile_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    autoTileMap.DrawTiles(0, [(Vector2I.Zero, (int)TILES.GRASS)]);
    GodotAccess.WaitNextFrames();
    var sourceId = autoTileMap.GetTileMapLayer(0).GetCellSourceId(Vector2I.Zero);

    // Then
    Assertions.AssertNotEqual(-1, sourceId);
    autoTileMap.QueueFree();
  }

  [TestMethod]
  public void DrawTilesAsync_PlacesTileMapTile_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    var task = autoTileMap.DrawTilesAsync(0, [(Vector2I.Zero, (int)TILES.GRASS)]);
    task.Wait();
    GodotAccess.WaitNextFrames();
    var sourceId = autoTileMap.GetTileMapLayer(0).GetCellSourceId(Vector2I.Zero);

    // Then
    Assertions.AssertNotEqual(-1, sourceId);
    autoTileMap.QueueFree();
  }

  [TestMethod]
  public void DrawTiles_RemovesTileMapTile_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    autoTileMap.DrawTiles(0, [(Vector2I.Zero, (int)TILES.GRASS)]);
    GodotAccess.WaitNextFrames();
    autoTileMap.DrawTiles(0, [(Vector2I.Zero, -1)]);
    GodotAccess.WaitNextFrames();
    var sourceId = autoTileMap.GetTileMapLayer(0).GetCellSourceId(Vector2I.Zero);

    // Then
    Assertions.AssertEqual(-1, sourceId);
    autoTileMap.QueueFree();
  }

  [TestMethod]
  public void DrawTilesAsync_RemovesTileMapTile_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    var task = autoTileMap.DrawTilesAsync(0, [(Vector2I.Zero, (int)TILES.GRASS)]);
    task.Wait();
    GodotAccess.WaitNextFrames();
    task = autoTileMap.DrawTilesAsync(0, [(Vector2I.Zero, -1)]);
    task.Wait();
    GodotAccess.WaitNextFrames();
    var sourceId = autoTileMap.GetTileMapLayer(0).GetCellSourceId(Vector2I.Zero);

    // Then
    Assertions.AssertEqual(-1, sourceId);
    autoTileMap.QueueFree();
  }

  [TestMethod]
  public void Clear_RemovesAllTileMapTiles_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    autoTileMap.DrawTiles(0, [(Vector2I.Zero, (int)TILES.GRASS)]);
    GodotAccess.WaitNextFrames();
    autoTileMap.Clear();
    GodotAccess.WaitNextFrames();
    var usedCells = autoTileMap.GetTileMapLayer(0).GetUsedCells();

    // Then
    Assertions.AssertEqual(0, usedCells.Count);
    autoTileMap.QueueFree();
  }

  [TestMethod]
  public void GetLayerCount_GetsExactLayerCount_WhenCalled()
  {
    // Given
    int shouldBeLayerCount = 1;
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(shouldBeLayerCount, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    var layerCount = autoTileMap.GetLayerCount();

    // Then
    Assertions.AssertEqual(shouldBeLayerCount, layerCount);
    autoTileMap.QueueFree();
  }
}