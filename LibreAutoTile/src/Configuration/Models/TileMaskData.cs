using System.Collections.Immutable;
using System.Text.Json;

namespace Qwaitumin.LibreAutoTile.Configuration.Models;

public class TileMaskData(ImmutableArray<int> mask, uint chance)
{
  public readonly ImmutableArray<int> Mask = mask;
  public readonly uint Chance = chance;

  public void Deconstruct(out ImmutableArray<int> Mask, out uint Chance)
  {
    Mask = this.Mask;
    Chance = this.Chance;
  }

  public static TileMaskData Construct(int[] mask, uint chance)
    => new([.. mask], chance);

  public override string ToString()
    => ToJsonString();

  public string ToJsonString()
    => JsonSerializer.Serialize(this, AutoTileJsonContext.Default.TileMaskData);

  public static TileMaskData? FromJsonString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileJsonContext.Default.TileMaskData)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public override bool Equals(object? obj)
  {
    if (obj is not TileMaskData other)
      return false;

    if (ReferenceEquals(this, other)) return true;
    if (other is null) return false;

    return Chance == other.Chance &&
      Mask.SequenceEqual(other.Mask);
  }

  public override int GetHashCode()
  {
    var hash = new HashCode();
    foreach (var value in Mask)
      hash.Add(value);
    hash.Add(Chance);
    return hash.ToHashCode();
  }
}