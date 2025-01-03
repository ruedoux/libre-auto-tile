using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests.Configuration;

[SimpleTestClass]
public class TileDefinitionTest
{
  readonly int[][] DEFAULT_TILEMASK = [[0, 0, 0, 0, 0, 0, 0, 0]];
  readonly TileMaskDefinition DEFAULT_TILEMASK_DEFINITION;
  readonly Dictionary<string, TileMaskDefinition> DEFAULT_FILE_TO_TILEMASK;


  public TileDefinitionTest()
  {
    DEFAULT_TILEMASK_DEFINITION = TileMaskDefinition.Construct(new() { { Vector2.Zero, DEFAULT_TILEMASK } });
    DEFAULT_FILE_TO_TILEMASK = new() { { "a", DEFAULT_TILEMASK_DEFINITION } };
  }

  [SimpleTestMethod]
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

  [SimpleTestMethod]
  public void Serialize_ShoudlKeepData_WhenDeserialized()
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