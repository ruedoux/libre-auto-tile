using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.Tiling;

/// <summary>
/// Fast <see cref="IEqualityComparer{T}"/> for <see cref="Vector2"/> that avoids
/// <see cref="HashCode.Combine"/>'s randomized hashing on the dirty-position hot path.
/// </summary>
internal sealed class Vector2FastComparer : IEqualityComparer<Vector2>
{
  public static readonly Vector2FastComparer INSTANCE = new();

  private Vector2FastComparer() { }

  public bool Equals(Vector2 x, Vector2 y)
    => x.X == y.X && x.Y == y.Y;

  public int GetHashCode(Vector2 obj)
    => (obj.X * 397) ^ obj.Y;
}
