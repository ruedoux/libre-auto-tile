using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Configuration;

[TestClass]
public class TileDefinitionTest
{
  private static readonly int[][] DEFAULT_TILEMASKS = [[0, 0, 0, 0, 0, 0, 0, 0]];
  private static readonly TileMaskDefinition DEFAULT_TILEMASK_DEFINITION;
  private static readonly Dictionary<string, TileMaskDefinition> DEFAULT_FILE_TO_TILEMASK;


  static TileDefinitionTest()
  {
    DEFAULT_TILEMASK_DEFINITION = TileMaskDefinition.Construct(new() { { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 1)] } });
    DEFAULT_FILE_TO_TILEMASK = new() { { "a", DEFAULT_TILEMASK_DEFINITION } };
  }

  [TestMethod]
  public void VerifyEquality()
  {
    SimpleEqualsVerifier.Verify(
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, name: "a"),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, name: "a"),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, name: "b"));
    SimpleEqualsVerifier.Verify(
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, color: new()),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, color: new()),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK, color: new(1, 2, 3)));
    SimpleEqualsVerifier.Verify(
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK),
      TileDefinition.Construct(imageFileNameToTileMaskDefinition: new() { { "b", DEFAULT_TILEMASK_DEFINITION } }));
  }

  [TestMethod]
  public void Serialize_ShouldKeepData_WhenDeserialized()
  {
    // Given
    TileDefinition tileDefinition = TileDefinition.Construct(
      imageFileNameToTileMaskDefinition: DEFAULT_FILE_TO_TILEMASK,
      color: new(1, 2, 3));

    // When
    var jsonString = tileDefinition.ToJsonString();
    var deserialized = TileDefinition.FromJsonString(jsonString);

    // Then
    Assertions.AssertEqual(tileDefinition, deserialized);
  }
}