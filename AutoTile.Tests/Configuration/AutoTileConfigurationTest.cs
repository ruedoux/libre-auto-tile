using System.Text.Json;
using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests.Configuration;

[SimpleTestClass]
public class AutoTileConfigurationTest
{
  const int TILE_SIZE = 16;
  const string AUTOTILE_MOCK_PATH = "../resources/AutoTileConfiguration.json";

  readonly int[][] DEFAULT_TILEMASK = [[0, 0, 0, 0, 0, 0, 0, 0]];
  readonly TileDefinition DEFAULT_TILE_DEFINITION;

  public AutoTileConfigurationTest()
  {
    DEFAULT_TILE_DEFINITION = TileDefinition.Construct(
      new() { { "", TileMaskDefinition.Construct(new() { { Vector3.Zero, DEFAULT_TILEMASK } }) } });
  }

  [SimpleTestMethod]
  public void VerifyEquality()
  {
    SimpleEqualsVerifier.Verify(
      AutoTileConfiguration.Construct(0, []),
      AutoTileConfiguration.Construct(0, []),
      AutoTileConfiguration.Construct(1, [])
    );
    SimpleEqualsVerifier.Verify(
      AutoTileConfiguration.Construct(0, new() { { 0, DEFAULT_TILE_DEFINITION } }),
      AutoTileConfiguration.Construct(0, new() { { 0, DEFAULT_TILE_DEFINITION } }),
      AutoTileConfiguration.Construct(0, new() { { 1, DEFAULT_TILE_DEFINITION } })
    );
  }

  [SimpleTestMethod]
  public void LoadObjectFromFile_ShouldDeserialize_WhenLoadedFromFile()
  {
    // Given
    // When
    string jsonString = File.ReadAllText(AUTOTILE_MOCK_PATH);
    var autoTileConfiguration = JsonSerializer.Deserialize(
      jsonString, AutoTileJsonContext.Default.AutoTileConfiguration);

    // Then
    Assertions.AssertNotNull(autoTileConfiguration);
    Assertions.AssertEqual(autoTileConfiguration.TileSize, TILE_SIZE);
  }

  [SimpleTestMethod]
  public void Serialize_ShoudlKeepData_WhenDeserialized()
  {
    // Given
    AutoTileConfiguration autoTileConfiguration = AutoTileConfiguration.Construct(
      0, new() { { 0, DEFAULT_TILE_DEFINITION } });

    // When
    var jsonString = autoTileConfiguration.ToJsonString();
    var deserialized = AutoTileConfiguration.FromJsonString(jsonString);

    // Then
    Assertions.AssertEqual(autoTileConfiguration, deserialized);
  }
}