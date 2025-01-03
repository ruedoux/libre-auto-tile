using System.Text;

namespace Qwaitumin.AutoTile;

public static class Bitmask
{
  public const byte DEFAULT = 0;
  public enum SurroundingDirection { TOP_LEFT = 0, TOP = 1, TOP_RIGHT = 2, RIGHT = 3, BOTTOM_RIGHT = 4, BOTTOM = 5, BOTTOM_LEFT = 6, LEFT = 7 }

  // NOTE: Before using it for getting tilemap atlas, parse it!
  public static byte Parse(byte bitmask = DEFAULT)
  {
    if (!(GetDirection(bitmask, SurroundingDirection.TOP) && GetDirection(bitmask, SurroundingDirection.LEFT)))
      bitmask = UpdateBitmask(bitmask, SurroundingDirection.TOP_LEFT, false);
    if (!(GetDirection(bitmask, SurroundingDirection.TOP) && GetDirection(bitmask, SurroundingDirection.RIGHT)))
      bitmask = UpdateBitmask(bitmask, SurroundingDirection.TOP_RIGHT, false);
    if (!(GetDirection(bitmask, SurroundingDirection.BOTTOM) && GetDirection(bitmask, SurroundingDirection.LEFT)))
      bitmask = UpdateBitmask(bitmask, SurroundingDirection.BOTTOM_LEFT, false);
    if (!(GetDirection(bitmask, SurroundingDirection.BOTTOM) && GetDirection(bitmask, SurroundingDirection.RIGHT)))
      bitmask = UpdateBitmask(bitmask, SurroundingDirection.BOTTOM_RIGHT, false);

    return bitmask;
  }

  public static byte UpdateBitmask(byte bitmaskValue, SurroundingDirection direction, bool connected)
    => connected ? (byte)(bitmaskValue | (1 << (byte)direction)) : (byte)(bitmaskValue & ~(1 << (byte)direction));

  public static byte FromArray(bool[] arr)
  {
    if (arr.Length != 8)
      throw new ArgumentException($"Bitmask array has to be 8 long but is {arr.Length}");

    byte bitmaskValue = DEFAULT;
    for (int i = 0; i < 8; i++)
      bitmaskValue = UpdateBitmask(bitmaskValue, (SurroundingDirection)i, arr[i]);

    return bitmaskValue;
  }

  public static bool GetDirection(int bitmask, SurroundingDirection direction)
    => (bitmask & (1 << (int)direction)) != 0;

  public static string AsString(int bitmask)
  {
    StringBuilder sb = new();
    sb.Append("{ ");

    foreach (SurroundingDirection direction in Enum.GetValues<SurroundingDirection>())
    {
      sb.Append($"{direction}: {GetDirection(bitmask, direction)}");
      if (direction != SurroundingDirection.LEFT)
        sb.Append(", ");
    }

    sb.Append(" }");
    return sb.ToString();
  }
}