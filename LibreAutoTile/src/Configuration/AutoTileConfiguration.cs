using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.Configuration;

public sealed class AutoTileConfiguration
{
  public uint? WildcardId { get; private set; }
  public uint TileSize { get; private set; }
  public ImmutableDictionary<uint, TileDefinition> TileDefinitions { get; private set; }

  [JsonConstructor]
  public AutoTileConfiguration(
    uint tileSize, ImmutableDictionary<uint, TileDefinition> tileDefinitions, uint? wildcardId = null)
  {
    TileSize = tileSize;
    TileDefinitions = tileDefinitions;
    WildcardId = wildcardId;
    Dictionary<string, uint> tileNameToTileIds = [];
    foreach (var (tileId, tileDefinition) in tileDefinitions)
      if (!tileNameToTileIds.TryAdd(tileDefinition.Name, tileId))
        throw new ArgumentException($"Same tile name for both tiles: '{tileId}' and '{tileNameToTileIds[tileDefinition.Name]}'");
  }

  public static AutoTileConfiguration Construct(
    uint tileSize, Dictionary<uint, TileDefinition> tileDefinitions, uint? wildcardId = null)
      => new(tileSize, tileDefinitions.ToImmutableDictionary(), wildcardId);

  public static AutoTileConfiguration? FromJsonString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileJsonContext.Default.AutoTileConfiguration)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public static AutoTileConfiguration LoadFromFile(string filePath)
  {
    var jsonString = File.ReadAllText(filePath);
    var autoTileConfiguration = FromJsonString(jsonString)
      ?? throw new ArgumentException("Invalid configuration file.");
    return autoTileConfiguration;
  }

  public void VerifyFiles()
  {
    foreach (var (tileId, tileDefinition) in TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(Path.Join(imageFileName)))
          throw new DirectoryNotFoundException($"Image does not exist: {imageFileName}");
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