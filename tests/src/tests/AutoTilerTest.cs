using System.Numerics;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class AutoTilerTest
{
  const byte TL = 1 << 0;
  const byte TT = 1 << 1;
  const byte TR = 1 << 2;
  const byte RR = 1 << 3;
  const byte BR = 1 << 4;
  const byte BB = 1 << 5;
  const byte BL = 1 << 6;
  const byte LL = 1 << 7;

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTile_WhenCalled()
  {
    // Given
    AutoTileData autoTileData = new(new bool[] { true }, new());
    AutoTiler autoTiler = new(1, new AutoTileData[] { autoTileData });

    // When
    autoTiler.PlaceTile(0, Vector2.Zero, 0);
    var tileData = autoTiler.GetTile(0, Vector2.Zero);
    autoTiler.PlaceTile(0, Vector2.Zero, -1);
    var tileDataAfterRemoval = autoTiler.GetTile(0, Vector2.Zero);

    // Then
    Assertions.AssertNull(tileDataAfterRemoval);
    Assertions.AssertNotNull(tileData);
    Assertions.AssertEqual(0, tileData.TileId);
    Assertions.AssertEqual(Bitmask.DEFAULT, tileData.Bitmask);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesTiles_WhenCalledAsync()
  {
    // Given
    AutoTileData autoTileData = new(new bool[] { true }, new());
    AutoTiler autoTiler = new(1, new AutoTileData[] { autoTileData });

    List<Vector2> positions = new();
    for (int x = 0; x < 10; x++)
      for (int y = 0; y < 10; y++)
        positions.Add(new(x, y));

    // When
    List<Task> tasks = new();
    for (int i = 0; i < 10; i++)
    {
      tasks.Add(new Task(() =>
      {
        foreach (var position in positions)
          autoTiler.PlaceTile(0, position, 0);
      }));
    }

    foreach (var task in tasks)
      task.Start();

    Task.WhenAll(tasks).Wait();

    // Then
    foreach (var position in positions)
    {
      var tileData = autoTiler.GetTile(0, position);
      Assertions.AssertNotNull(tileData);
      Assertions.AssertEqual(0, tileData.TileId);
    }
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyUpdatesBitmaskForSingleTile_WhenConnection()
  {
    // Given
    AutoTileData autoTileData = new(new bool[] { true }, new());
    AutoTiler autoTiler = new(1, new AutoTileData[] { autoTileData });
    HelperAutoTiler helperAutoTiler = new(autoTiler, 0, Vector2.Zero);

    // When
    //Then
    autoTiler.PlaceTile(0, Vector2Directions.Zero, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { 0, 0, 0,
                   0, 0, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.TopLeft, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { BR, 0, 0,
                   0, TL, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.Top, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { BR | RR, LL | BB, 0,
                   0, TL | TT, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.TopRight, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { RR | BR, LL | BB | RR, LL | BL,
                   0, TL | TT | TR, 0,
                   0, 0, 0 });
  }


  [SimpleTestMethod]
  public void PlaceTile_CorrectlyUpdatesBitmaskForMultipleTiles_WhenConnection()
  {
    // Given
    AutoTileData autoTileData1 = new(new bool[] { true, true }, new());
    AutoTileData autoTileData2 = new(new bool[] { true, true }, new());
    AutoTiler autoTiler = new(1, new AutoTileData[] { autoTileData1, autoTileData2 });
    HelperAutoTiler helperAutoTiler = new(autoTiler, 0, Vector2.Zero);

    // When
    //Then
    autoTiler.PlaceTile(0, Vector2Directions.Zero, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { 0, 0, 0,
                   0, 0, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.TopLeft, 1);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { BR, 0, 0,
                   0, TL, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.Top, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { BR | RR, LL | BB, 0,
                   0, TL | TT, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.TopRight, 1);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { RR | BR, LL | BB | RR, LL | BL,
                   0, TL | TT | TR, 0,
                   0, 0, 0 });
  }

  [SimpleTestMethod]
  public void PlaceTile_DoesntUpdateBitmaskForMultipleTiles_WhenNoConnection()
  {
    // Given
    AutoTileData autoTileData1 = new(new bool[] { true, false }, new());
    AutoTileData autoTileData2 = new(new bool[] { false, true }, new());
    AutoTiler autoTiler = new(1, new AutoTileData[] { autoTileData1, autoTileData2 });
    HelperAutoTiler helperAutoTiler = new(autoTiler, 0, Vector2Directions.Zero);

    // When
    //Then
    autoTiler.PlaceTile(0, Vector2Directions.Zero, 0);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { 0, 0, 0,
                   0, 0, 0,
                   0, 0, 0 });

    autoTiler.PlaceTile(0, Vector2Directions.TopLeft, 1);
    helperAutoTiler.AssertBitmaskMatches(
      new byte[] { 0, 0, 0,
                   0, 0, 0,
                   0, 0, 0 });
  }

  class HelperAutoTiler
  {
    private readonly AutoTiler autoTiler;
    private readonly int layer;
    private readonly Vector2 atPosition;

    public HelperAutoTiler(AutoTiler autoTiler, int layer, Vector2 atPosition)
    {
      this.autoTiler = autoTiler;
      this.layer = layer;
      this.atPosition = atPosition;
    }

    public void AssertBitmaskMatches(byte[] bitmasks)
    {
      Assertions.AssertEqual(9, bitmasks.Length);

      var middleTile = autoTiler.GetTile(layer, atPosition);
      var topLeftTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.TopLeft);
      var topTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.Top);
      var topRightTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.TopRight);
      var rightTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.Right);
      var bottomRightTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.BottomRight);
      var bottomTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.Bottom);
      var bottomLeftTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.BottomLeft);
      var leftTile = autoTiler.GetTile(layer, atPosition + Vector2Directions.Left);

      VerifyTile(topLeftTile, bitmasks[0]);
      VerifyTile(topTile, bitmasks[1]);
      VerifyTile(topRightTile, bitmasks[2]);
      VerifyTile(leftTile, bitmasks[3]);
      VerifyTile(middleTile, bitmasks[4]);
      VerifyTile(rightTile, bitmasks[5]);
      VerifyTile(bottomLeftTile, bitmasks[6]);
      VerifyTile(bottomTile, bitmasks[7]);
      VerifyTile(bottomRightTile, bitmasks[8]);
    }

    private static void VerifyTile(TileData? tileData, byte bitmaskValue)
    {
      if (tileData is null) Assertions.AssertEqual(0, bitmaskValue);
      else Assertions.AssertEqual(bitmaskValue, tileData.Bitmask);
    }
  }
}