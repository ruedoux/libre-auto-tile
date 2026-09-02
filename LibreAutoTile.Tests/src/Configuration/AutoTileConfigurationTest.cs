using System.Text.Json;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Configuration;

[TestClass]
public class AutoTileConfigurationTest
{
  private const uint TILE_SIZE = 16;
  private const string AUTOTILE_MOCK_PATH = "../resources/configurations/ExampleConfigurationTransient.json";

  private static readonly int[][] DEFAULT_TILEMASK = [[0, 0, 0, 0, 0, 0, 0, 0]];
  private static readonly TileDefinition DEFAULT_TILE_DEFINITION;

  static AutoTileConfigurationTest()
  {
    DEFAULT_TILE_DEFINITION = TileDefinition.Construct(
      new()
      {
        {
          "resources/mock.jpg",
          TileMaskDefinition.Construct(new() {{ Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASK[0], 1)] }})
        }
      });
  }

  [TestMethod]
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

  [TestMethod]
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

  [TestMethod]
  public void Serialize_ShouldKeepData_WhenDeserialized()
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