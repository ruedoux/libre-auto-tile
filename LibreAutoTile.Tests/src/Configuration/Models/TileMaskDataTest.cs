using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Configuration;

[TestClass]
public class TileMaskDataTest
{
  private static readonly int[] DEFAULT_TILEMASK = [0, 0, 0, 0, 0, 0, 0, 0];

  [TestMethod]
  public void VerifyEquality()
  {
    SimpleEqualsVerifier.Verify(
      TileMaskData.Construct(DEFAULT_TILEMASK, 0),
      TileMaskData.Construct(DEFAULT_TILEMASK, 0),
      TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 1], 0));
    SimpleEqualsVerifier.Verify(
      TileMaskData.Construct(DEFAULT_TILEMASK, 0),
      TileMaskData.Construct(DEFAULT_TILEMASK, 0),
      TileMaskData.Construct(DEFAULT_TILEMASK, 1));
  }

  [TestMethod]
  public void Serialize_ShouldKeepData_WhenDeserialized()
  {
    // Given
    TileMaskData tileMaskData = TileMaskData.Construct(DEFAULT_TILEMASK, 0);

    // When
    var jsonString = tileMaskData.ToString();
    var deserialized = TileMaskData.FromJsonString(jsonString);

    // Then
    Assertions.AssertEqual(tileMaskData, deserialized);
  }
}