using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

public readonly struct TileAtlas(Vector2 position, string imageFileName, int chance = int.MaxValue)
{
  public readonly Vector2 Position { get; init; } = position;
  public readonly string ImageFileName { get; init; } = imageFileName;

  /// <summary>
  /// Chance for this TileAtlas to appear if there are more than one.
  /// Should be caluculated as: 60% = int.MaxValue * 0.60
  /// </summary>
  public readonly int Chance { get; init; } = chance;

  public bool Equals(TileAtlas other)
    => Position.Equals(other.Position)
      && string.Equals(ImageFileName, other.ImageFileName, StringComparison.Ordinal)
      && Chance == other.Chance;

  public override bool Equals(object? obj)
      => obj is TileAtlas other && Equals(other);

  public override int GetHashCode()
      => HashCode.Combine(Position, ImageFileName, Chance);

  public override string ToString() => $"({Position}, {ImageFileName}, {Chance})";

  public static bool operator ==(TileAtlas left, TileAtlas right)
    => left.Equals(right);

  public static bool operator !=(TileAtlas left, TileAtlas right)
    => !(left == right);
}