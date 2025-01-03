using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class BitmaskTest
{
  private readonly byte[] relevantBitmaskBytes = new byte[47] { 0, 2, 8, 10, 14, 32, 34, 40, 42, 46, 56, 58, 62, 128, 130, 131, 136, 138, 139, 142, 143, 160, 162, 163, 168, 170, 171, 174, 175, 184, 186, 187, 190, 191, 224, 226, 227, 232, 234, 235, 238, 239, 248, 250, 251, 254, 255 };

  [SimpleTestMethod]
  public void Parse_ShouldCorrectlyParse_WhenRelevantValuePassed()
  {
    // Given
    // When
    for (int i = 0; i < relevantBitmaskBytes.Length; i++)
    {
      byte expectedByte = relevantBitmaskBytes[i];
      byte bitmask = Bitmask.Parse(expectedByte);

      // Then
      Assertions.AssertEqual(expectedByte, bitmask);
    }
  }

  [SimpleTestMethod]
  public void Parse_ShouldRemoveIrrelevantDirection_WhenCalled()
  {
    // Given
    byte bitmaskDontRemove = Bitmask.UpdateBitmask(Bitmask.DEFAULT, Bitmask.SurroundingDirection.TOP_LEFT, true);
    byte bitmaskRemove = Bitmask.UpdateBitmask(Bitmask.DEFAULT, Bitmask.SurroundingDirection.TOP_LEFT, true);

    bitmaskDontRemove = Bitmask.UpdateBitmask(bitmaskDontRemove, Bitmask.SurroundingDirection.LEFT, true);
    bitmaskDontRemove = Bitmask.UpdateBitmask(bitmaskDontRemove, Bitmask.SurroundingDirection.TOP, true);

    // When
    byte bitmaskDontRemoveAfter = Bitmask.Parse(bitmaskDontRemove);
    byte bitmaskRemoveAfter = Bitmask.Parse(bitmaskRemove);

    // Then
    Assertions.AssertEqual(bitmaskDontRemove, bitmaskDontRemoveAfter);
    Assertions.AssertNotEqual(bitmaskRemove, bitmaskRemoveAfter);
    Assertions.AssertEqual(Bitmask.DEFAULT, bitmaskRemoveAfter);
  }

  [SimpleTestMethod]
  public void UpdateBitmask_ShouldReturnValidDirection_WhenCalled()
  {
    // Given
    var bitmask = Bitmask.DEFAULT;
    var directions = Enum.GetValues<Bitmask.SurroundingDirection>();

    // When
    // Then
    foreach (var direction in directions)
    {
      bitmask = Bitmask.UpdateBitmask(bitmask, direction, true);
      byte expectedByte = Bitmask.DEFAULT;
      for (int index = 0; index < (int)direction + 1; index++)
        expectedByte = (byte)(expectedByte | 1 << index);

      Assertions.AssertEqual(expectedByte, bitmask);
    }

    foreach (var direction in directions)
    {
      bitmask = Bitmask.UpdateBitmask(bitmask, direction, false);
      byte expectedByte = Byte.MaxValue;
      for (int index = 0; index < (int)direction + 1; index++)
        expectedByte = (byte)(expectedByte & ~(1 << index));

      Assertions.AssertEqual(expectedByte, bitmask);
    }
  }
}