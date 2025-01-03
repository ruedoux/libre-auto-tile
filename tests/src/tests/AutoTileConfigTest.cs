using System.Numerics;
using System.Text.Json;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class AutoTileConfigTest
{
  const int TILE_SIZE = 16;

  const string SINGLE_TILE_NAME1 = "ExampleTileName3";
  const string SINGLE_TILE_NAME2 = "ExampleTileName4";
  const string SINGLE_TILE_FILE = "ExampleSingle.png";
  const string SINGLE_TILE_BITMASK = "SingleBitmaskName";

  const string AUTO_TILE_NAME1 = "ExampleTileName";
  const string AUTO_TILE_NAME2 = "ExampleTileName2";
  const string AUTO_TILE_FILE = "ExampleAutoTiled.jpg";
  const string AUTO_TILE_BITMASK = "ExampleBitmaskName";

  const string TILESET_MOCK_PATH = "./resources/AutoTileConfig.json";

  [SimpleTestMethod]
  public void LoadObjectFromFile_ShouldDeserialize_WhenLoadedFromFile()
  {
    // Given
    // When
    string jsonString = File.ReadAllText(TILESET_MOCK_PATH);
    var autoTileConfig = JsonSerializer.Deserialize(
      jsonString, AutoTileConfigJsonContext.Default.AutoTileConfig);

    // Then
    Assertions.AssertNotNull(autoTileConfig);
    Assertions.AssertEqual(autoTileConfig.TileSize, TILE_SIZE);

    Assertions.AssertTrue(autoTileConfig.BitmaskSets.ContainsKey(AUTO_TILE_BITMASK));
    Assertions.AssertEqual(Vector2.Zero, autoTileConfig.BitmaskSets[AUTO_TILE_BITMASK][56]);
    Assertions.AssertTrue(autoTileConfig.BitmaskSets.ContainsKey(SINGLE_TILE_BITMASK));
    Assertions.AssertEqual(Vector2.Zero, autoTileConfig.BitmaskSets[SINGLE_TILE_BITMASK][0]);

    Assertions.AssertEqual(
      new TileDefinition(Layer: 0, ImageFileName: SINGLE_TILE_FILE, AutoTileGroup: -1, BitmaskName: SINGLE_TILE_BITMASK, PositionInSet: Vector2.Zero),
      autoTileConfig.TileDefinitions[SINGLE_TILE_NAME1]);
    Assertions.AssertEqual(
      new TileDefinition(Layer: 0, ImageFileName: SINGLE_TILE_FILE, AutoTileGroup: -1, BitmaskName: SINGLE_TILE_BITMASK, PositionInSet: new(TILE_SIZE, 0)),
      autoTileConfig.TileDefinitions[SINGLE_TILE_NAME2]);
    Assertions.AssertEqual(
      new TileDefinition(Layer: 0, ImageFileName: AUTO_TILE_FILE, AutoTileGroup: 0, BitmaskName: AUTO_TILE_BITMASK, PositionInSet: Vector2.Zero),
      autoTileConfig.TileDefinitions[AUTO_TILE_NAME1]);
    Assertions.AssertEqual(
      new TileDefinition(Layer: 0, ImageFileName: AUTO_TILE_FILE, AutoTileGroup: 0, BitmaskName: AUTO_TILE_BITMASK, PositionInSet: new(TILE_SIZE * 6, 0)),
      autoTileConfig.TileDefinitions[AUTO_TILE_NAME2]);
  }
}