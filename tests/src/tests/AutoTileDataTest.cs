using System.Numerics;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class AutoTileDataTest
{
  [SimpleTestMethod]
  public void CanConnectTo_CorrectlyReturnsConnections_WhenSetup()
  {
    // Given
    AutoTileData autoTileData = new(new bool[] { false, true }, new());

    // When
    var shouldNotConnect = autoTileData.CanConnectTo(0);
    var shouldConnect = autoTileData.CanConnectTo(1);

    // Then
    Assertions.AssertTrue(shouldConnect);
    Assertions.AssertFalse(shouldNotConnect);
  }

  [SimpleTestMethod]
  public void GetAtlasCoords_CorrectlyReturnCoords_WhenGivenCorrectPosition()
  {
    // Given
    AutoTileData autoTileData = new(new bool[] { true }, new() { { 2, Vector2.One } });

    // When
    var shouldBeOne = autoTileData.GetAtlasCoords(2);
    var shouldBeDefault = autoTileData.GetAtlasCoords(123);

    // Then
    Assertions.AssertEqual(Vector2.One, shouldBeOne);
    Assertions.AssertEqual(Vector2.Zero, shouldBeDefault);
  }
}