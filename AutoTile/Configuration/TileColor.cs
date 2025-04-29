namespace Qwaitumin.AutoTile.Configuration;

public readonly struct TileColor(byte r, byte g, byte b, byte a = 255)
{
  public readonly byte R = r;
  public readonly byte G = g;
  public readonly byte B = b;
  public readonly byte A = a;

  public float Rf => ((float)R) / byte.MaxValue;
  public float Gf => ((float)G) / byte.MaxValue;
  public float Bf => ((float)B) / byte.MaxValue;
  public float Af => ((float)A) / byte.MaxValue;

  public static bool operator ==(TileColor left, TileColor right)
  => left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A;

  public static bool operator !=(TileColor left, TileColor right)
    => !(left == right);

  public override readonly bool Equals(object? obj)
  {
    if (obj is not TileColor)
      return false;

    TileColor other = (TileColor)obj;
    return this == other;
  }

  public override readonly int GetHashCode()
    => HashCode.Combine(R, G, B, A);
}