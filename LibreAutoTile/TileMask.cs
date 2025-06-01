namespace Qwaitumin.LibreAutoTile;

public readonly struct TileMask
{
  public enum SurroundingDirection { TopLeft = 0, Top = 1, TopRight = 2, Right = 3, BottomRight = 4, Bottom = 5, BottomLeft = 6, Left = 7 }

  public readonly int TopLeft { get; init; } = -1;
  public readonly int Top { get; init; } = -1;
  public readonly int TopRight { get; init; } = -1;
  public readonly int Right { get; init; } = -1;
  public readonly int BottomRight { get; init; } = -1;
  public readonly int Bottom { get; init; } = -1;
  public readonly int BottomLeft { get; init; } = -1;
  public readonly int Left { get; init; } = -1;

  public TileMask() { }

  public TileMask(
    int topLeft = -1,
    int top = -1,
    int topRight = -1,
    int right = -1,
    int bottomRight = -1,
    int bottom = -1,
    int bottomLeft = -1,
    int left = -1) : this()
  {
    TopLeft = topLeft;
    Top = top;
    TopRight = topRight;
    Right = right;
    BottomRight = bottomRight;
    Bottom = bottom;
    BottomLeft = bottomLeft;
    Left = left;
  }

  public static TileMask GetZero()
    => new(0, 0, 0, 0, 0, 0, 0, 0);

  public static TileMask GetMinusOne()
    => new(-1, -1, -1, -1, -1, -1, -1, -1);

  public static TileMask FromArray(int[] arr)
  {
    if (arr.Length != 8)
      throw new ArgumentException($"TileMask array should be of size 8 but is: '{arr.Length}'");

    return new TileMask(
      topLeft: arr[0],
      top: arr[1],
      topRight: arr[2],
      right: arr[3],
      bottomRight: arr[4],
      bottom: arr[5],
      bottomLeft: arr[6],
      left: arr[7]);
  }

  public static TileMask ConstructModified(
    TileMask tileMask,
    SurroundingDirection surroundingDirection,
    int tileId)
  {
    int topLeft = tileMask.TopLeft;
    int top = tileMask.Top;
    int topRight = tileMask.TopRight;
    int right = tileMask.Right;
    int bottomRight = tileMask.BottomRight;
    int bottom = tileMask.Bottom;
    int bottomLeft = tileMask.BottomLeft;
    int left = tileMask.Left;

    switch ((int)surroundingDirection)
    {
      case 0: topLeft = tileId; break;
      case 1: top = tileId; break;
      case 2: topRight = tileId; break;
      case 3: right = tileId; break;
      case 4: bottomRight = tileId; break;
      case 5: bottom = tileId; break;
      case 6: bottomLeft = tileId; break;
      case 7: left = tileId; break;
    }

    return new(
      topLeft: topLeft,
      top: top,
      topRight: topRight,
      right: right,
      bottomRight: bottomRight,
      bottom: bottom,
      bottomLeft: bottomLeft,
      left: left);
  }

  public readonly int[] ToArray()
    => [TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left];

  public override readonly int GetHashCode()
    => HashCode.Combine(TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left);

  public override readonly bool Equals(object? obj)
  {
    if (obj is TileMask other)
    {
      return TopLeft == other.TopLeft &&
             Top == other.Top &&
             TopRight == other.TopRight &&
             Right == other.Right &&
             BottomRight == other.BottomRight &&
             Bottom == other.Bottom &&
             BottomLeft == other.BottomLeft &&
             Left == other.Left;
    }
    return false;
  }

  public static bool operator ==(TileMask left, TileMask right)
    => left.Equals(right);

  public static bool operator !=(TileMask left, TileMask right)
    => !(left == right);

  public override readonly string ToString()
    => $"[{TopLeft},{Top},{TopRight},{Right},{BottomRight},{Bottom},{BottomLeft},{Left}]";
}