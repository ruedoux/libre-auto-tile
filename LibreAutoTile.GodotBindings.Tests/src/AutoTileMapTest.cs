using Qwaitumin.SimpleTest;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.Configuration;
using System.Numerics;
using Godot;

namespace LibreAutoTile.GodotBindings.Tests;

[SimpleTestClass]
public class AutoTileMapTest
{
  enum TILES { GRASS = 0, WATER = 1 }
  const string CONFIG_PATH = "../resources/configurations/ExampleConfigurationTransient.json";


  [SimpleTestMethod]
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

  [SimpleTestMethod]
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

  [SimpleTestMethod]
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

  [SimpleTestMethod]
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

  [SimpleTestMethod]
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

  [SimpleTestMethod]
  public void GetLayerCount_GetsExactLayerCount_WhenCalled()
  {
    // Given
    uint shouldBeLayerCount = 1;
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(shouldBeLayerCount, autoTileConfiguration);
    GodotAccess.AddNodeToTree(autoTileMap);

    // When
    var layerCount = autoTileMap.GetLayerCount();

    // Then
    Assertions.AssertEqual((int)shouldBeLayerCount, layerCount);
    autoTileMap.QueueFree();
  }
}