namespace Qwaitumin.LibreAutoTile.Configuration;

public readonly struct Vector3(int x, int y, int z)
{
  public readonly int X = x;
  public readonly int Y = y;
  public readonly int Z = z;

  public static Vector3 Zero => new(0, 0, 0);
  public static Vector3 One => new(1, 1, 1);

  public readonly void Deconstruct(out int x, out int y, out int z)
  {
    x = X;
    y = Y;
    z = Z;
  }

  public static Vector3 operator +(Vector3 left, Vector3 right)
    => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

  public static Vector3 operator -(Vector3 left, Vector3 right)
    => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

  public static Vector3 operator -(Vector3 vec)
    => new(-vec.X, -vec.Y, -vec.Z);

  public static Vector3 operator *(Vector3 vec, int scale)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale);

  public static Vector3 operator *(int scale, Vector3 vec)
    => new(vec.X * scale, vec.Y * scale, vec.Z * scale);

  public static Vector3 operator *(Vector3 left, Vector3 right)
    => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

  public static Vector3 operator /(Vector3 vec, int divisor)
    => new(vec.X / divisor, vec.Y / divisor, vec.Z / divisor);

  public static Vector3 operator /(Vector3 vec, Vector3 divisorv)
    => new(vec.X / divisorv.X, vec.Y / divisorv.Y, vec.Z / divisorv.Z);

  public static Vector3 operator %(Vector3 vec, int divisor)
    => new(vec.X % divisor, vec.Y % divisor, vec.Z % divisor);

  public static Vector3 operator %(Vector3 vec, Vector3 divisorv)
    => new(vec.X % divisorv.X, vec.Y % divisorv.Y, vec.Z % divisorv.Z);

  public static bool operator ==(Vector3 left, Vector3 right)
    => left.Equals(right);

  public static bool operator !=(Vector3 left, Vector3 right)
    => !left.Equals(right);

  public static Vector3 From(Vector2 v, int z)
   => new(v.X, v.Y, z);

  public Vector2 ToVector2()
   => new(X, Y);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not Vector3 other)
      return false;

    return X == other.X && Y == other.Y && Z == other.Z;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(X, Y, Z);

  public override readonly string ToString()
    => $"({X},{Y},{Z})";

  public static Vector3 FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 3)
      throw new ArgumentException($"Unable to convert string to Vector3: {text}");
    return new Vector3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
  }
}