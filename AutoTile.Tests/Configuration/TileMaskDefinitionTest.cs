using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests.Configuration;

[SimpleTestClass]
public class TileMaskDefinitionTest
{
  readonly int[][] DEFAULT_TILEMASK = [[0, 0, 0, 0, 0, 0, 0, 0]];

  [SimpleTestMethod]
  public void VerifyEquality()
  {
    SimpleEqualsVerifier.Verify(
      TileMaskDefinition.Construct(new() { { Vector3.Zero, DEFAULT_TILEMASK } }),
      TileMaskDefinition.Construct(new() { { Vector3.Zero, DEFAULT_TILEMASK } }),
      TileMaskDefinition.Construct(new() { { Vector3.One, DEFAULT_TILEMASK } }));
    SimpleEqualsVerifier.Verify(
      TileMaskDefinition.Construct(new() { { Vector3.Zero, DEFAULT_TILEMASK } }),
      TileMaskDefinition.Construct(new() { { Vector3.Zero, DEFAULT_TILEMASK } }),
      TileMaskDefinition.Construct(new() { { Vector3.Zero, [[0, 0, 0, 0, 0, 0, 0, 1]] } }));
  }

  [SimpleTestMethod]
  public void Serialize_ShoudlKeepData_WhenDeserialized()
  {
    // Given
    TileMaskDefinition tileMaskDefinition = TileMaskDefinition.Construct(
      new() { { Vector3.Zero, DEFAULT_TILEMASK } });

    // When
    var jsonString = tileMaskDefinition.ToJsonString();
    var deserialized = TileMaskDefinition.FromJsonString(jsonString);

    // Then
    Assertions.AssertEqual(tileMaskDefinition, deserialized);
  }
}