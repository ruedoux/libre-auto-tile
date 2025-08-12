using System.Collections.Immutable;
using System.Text.Json;

namespace Qwaitumin.LibreAutoTile.Configuration.Models;

public sealed class TileDefinition(
  ImmutableDictionary<string, TileMaskDefinition> imageFileNameToTileMaskDefinition,
  string name = TileDefinition.DEFAULT_STRING,
  TileColor color = default,
  int connectionGroup = -1)
{
  const string DEFAULT_STRING = "<NONE>";

  public readonly ImmutableDictionary<string, TileMaskDefinition> ImageFileNameToTileMaskDefinition = imageFileNameToTileMaskDefinition;
  public readonly string Name = name;
  public readonly TileColor Color = color;
  public readonly int ConnectionGroup = connectionGroup;

  public static TileDefinition Construct(
    Dictionary<string, TileMaskDefinition> imageFileNameToTileMaskDefinition,
    string name = DEFAULT_STRING,
    TileColor color = default,
    int connectionGroup = -1)
      => new(
        imageFileNameToTileMaskDefinition: imageFileNameToTileMaskDefinition.ToImmutableDictionary(),
        name: name,
        color: color,
        connectionGroup: connectionGroup);

  public static TileDefinition? FromJsonString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileJsonContext.Default.TileDefinition)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public string ToJsonString()
    => JsonSerializer.Serialize(this, AutoTileJsonContext.Default.TileDefinition);

  public override bool Equals(object? obj)
  {
    if (obj is not TileDefinition other)
      return false;

    bool dictsEqual = ImageFileNameToTileMaskDefinition.Count == other.ImageFileNameToTileMaskDefinition.Count;
    if (!dictsEqual)
      return false;

    foreach (var (imageFileName, tileMaskDefinition) in ImageFileNameToTileMaskDefinition)
    {
      if (!other.ImageFileNameToTileMaskDefinition.TryGetValue(imageFileName, out var othertileMaskDefinition))
        return false;
      if (!othertileMaskDefinition.Equals(tileMaskDefinition))
        return false;
    }

    return Name == other.Name && Color == other.Color;
  }

  public override int GetHashCode()
  {
    var hash = new HashCode();
    hash.Add(Name);
    hash.Add(Color);
    foreach (var (k, v) in ImageFileNameToTileMaskDefinition)
    {
      hash.Add(k);
      hash.Add(v);
    }
    return hash.ToHashCode();
  }
}