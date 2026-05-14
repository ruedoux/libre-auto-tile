using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Configuration;

[SimpleTestClass]
public class TileMaskDefinitionTest
{
  private static readonly int[][] DEFAULT_TILEMASKS = [[0, 0, 0, 0, 0, 0, 0, 0]];

  [SimpleTestMethod]
  public void VerifyEquality()
  {
    SimpleEqualsVerifier.Verify(
      TileMaskDefinition.Construct(new()
      {
      { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      }),
      TileMaskDefinition.Construct(new()
      {
      { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      }),
      TileMaskDefinition.Construct(new()
      {
      { Vector3.One, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      }));

    SimpleEqualsVerifier.Verify(
      TileMaskDefinition.Construct(new()
      {
      { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      }),
      TileMaskDefinition.Construct(new()
      {
      { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      }),
      TileMaskDefinition.Construct(new()
      {
      { Vector3.Zero, [TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 1], 0)] }
      }));
  }

  [SimpleTestMethod]
  public void Serialize_ShoudlKeepData_WhenDeserialized()
  {
    // Given
    TileMaskDefinition tileMaskDefinition = TileMaskDefinition.Construct(
      new()
      {
      { Vector3.Zero, [TileMaskData.Construct(DEFAULT_TILEMASKS[0], 0)] }
      });

    // When
    var jsonString = tileMaskDefinition.ToJsonString();
    var deserialized = TileMaskDefinition.FromJsonString(jsonString);

    // Then
    Assertions.AssertEqual(tileMaskDefinition, deserialized);
  }
}