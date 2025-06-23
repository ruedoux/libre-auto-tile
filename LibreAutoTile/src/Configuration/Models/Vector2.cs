namespace Qwaitumin.LibreAutoTile.Configuration.Models;

public readonly struct Vector2(int x, int y)
{
  public readonly int X = x;
  public readonly int Y = y;

  public static Vector2 Zero => new(0, 0);
  public static Vector2 One => new(1, 1);

  public static Vector2 TopLeft => new(-1, -1);
  public static Vector2 Top => new(0, -1);
  public static Vector2 TopRight => new(1, -1);
  public static Vector2 Right => new(1, 0);
  public static Vector2 BottomRight => new(1, 1);
  public static Vector2 Bottom => new(0, 1);
  public static Vector2 BottomLeft => new(-1, 1);
  public static Vector2 Left => new(-1, 0);

  public readonly void Deconstruct(out int x, out int y)
  {
    x = X;
    y = Y;
  }

  public static Vector2 operator +(Vector2 left, Vector2 right)
    => new(left.X + right.X, left.Y + right.Y);

  public static Vector2 operator -(Vector2 left, Vector2 right)
    => new(left.X - right.X, left.Y - right.Y);

  public static Vector2 operator -(Vector2 vec)
    => new(-vec.X, -vec.Y);

  public static Vector2 operator *(Vector2 vec, int scale)
    => new(vec.X * scale, vec.Y * scale);

  public static Vector2 operator *(int scale, Vector2 vec)
    => new(vec.X * scale, vec.Y * scale);

  public static Vector2 operator *(Vector2 left, Vector2 right)
    => new(left.X * right.X, left.Y * right.Y);

  public static Vector2 operator /(Vector2 vec, int divisor)
    => new(vec.X / divisor, vec.Y / divisor);

  public static Vector2 operator /(Vector2 vec, Vector2 divisorv)
    => new(vec.X / divisorv.X, vec.Y / divisorv.Y);

  public static Vector2 operator %(Vector2 vec, int divisor)
    => new(vec.X % divisor, vec.Y % divisor);

  public static Vector2 operator %(Vector2 vec, Vector2 divisorv)
    => new(vec.X % divisorv.X, vec.Y % divisorv.Y);

  public static bool operator ==(Vector2 left, Vector2 right)
    => left.Equals(right);

  public static bool operator !=(Vector2 left, Vector2 right)
    => !left.Equals(right);

  public static Vector2 From(Vector3 v)
   => new(v.X, v.Y);

  public Vector3 ToVector3(int z)
   => new(X, Y, z);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not Vector2 other)
      return false;

    return X == other.X && Y == other.Y;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(X, Y);

  public override readonly string ToString()
    => $"({X},{Y})";

  public static Vector2 FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 2)
      throw new ArgumentException($"Unable to convert string to Vector2: {text}");
    return new Vector2(int.Parse(parts[0]), int.Parse(parts[1]));
  }
}