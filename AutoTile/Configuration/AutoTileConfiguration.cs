using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qwaitumin.AutoTile.Configuration;

public sealed class AutoTileConfiguration
{
  public int TileSize { get; private set; }
  public ImmutableDictionary<uint, TileDefinition> TileDefinitions { get; private set; }

  [JsonConstructor]
  public AutoTileConfiguration(
    int tileSize, ImmutableDictionary<uint, TileDefinition> tileDefinitions)
  {
    TileSize = tileSize;
    TileDefinitions = tileDefinitions;

    Dictionary<string, uint> tileNameToTileIds = [];
    foreach (var (tileId, tileDefinition) in tileDefinitions)
      if (!tileNameToTileIds.TryAdd(tileDefinition.Name, tileId))
        throw new ArgumentException($"Same tile name for both tiles: '{tileId}' and '{tileNameToTileIds[tileDefinition.Name]}'");
  }

  public static AutoTileConfiguration Construct(
    int tileSize, Dictionary<uint, TileDefinition> tileDefinitions)
      => new(tileSize, tileDefinitions.ToImmutableDictionary());

  public static AutoTileConfiguration? FromJsonString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileJsonContext.Default.AutoTileConfiguration)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public string ToJsonString()
    => JsonSerializer.Serialize(this, AutoTileJsonContext.Default.AutoTileConfiguration);

  public override string ToString()
    => ToJsonString();

  public bool Equals(AutoTileConfiguration? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;

    return TileSize == other.TileSize && TileDefinitions.SequenceEqual(other.TileDefinitions);
  }

  public override bool Equals(object? obj) => Equals(obj as AutoTileConfiguration);

  public override int GetHashCode()
  {
    int hash = TileSize.GetHashCode();
    foreach (var pair in TileDefinitions)
      hash = HashCode.Combine(hash, pair.Key.GetHashCode(), pair.Value.GetHashCode());

    return hash;
  }
}