using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.Configuration;

public class AutoTileConfiguration
{
  public uint? WildcardId { get; private set; }
  public uint TileSize { get; private set; }
  public TileShape TileShape { get; private set; }
  public ImmutableDictionary<uint, TileDefinition> TileDefinitions { get; private set; }

  [JsonConstructor]
  public AutoTileConfiguration(
    uint tileSize,
    ImmutableDictionary<uint, TileDefinition> tileDefinitions,
    uint? wildcardId = null,
    TileShape tileShape = TileShape.Square)
  {
    TileSize = tileSize;
    TileDefinitions = tileDefinitions;
    WildcardId = wildcardId;
    TileShape = tileShape;
    Dictionary<string, uint> tileNameToTileIds = [];
    foreach (var (tileId, tileDefinition) in tileDefinitions)
      if (!tileNameToTileIds.TryAdd(tileDefinition.Name, tileId))
        throw new ArgumentException($"Same tile name for both tiles: '{tileId}' and '{tileNameToTileIds[tileDefinition.Name]}'");
  }

  public static AutoTileConfiguration Construct(
    uint tileSize,
    Dictionary<uint, TileDefinition> tileDefinitions,
    uint? wildcardId = null,
    TileShape tileShape = TileShape.Square)
      => new(tileSize, tileDefinitions.ToImmutableDictionary(), wildcardId, tileShape);

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

    return TileSize == other.TileSize
      && TileShape == other.TileShape
      && WildcardId == other.WildcardId
      && TileDefinitionsEqual(TileDefinitions, other.TileDefinitions);
  }

  public override bool Equals(object? obj) => Equals(obj as AutoTileConfiguration);

  public override int GetHashCode()
  {
    var hash = new HashCode();
    hash.Add(TileSize);
    hash.Add(TileShape);
    hash.Add(WildcardId);

    int tileDefinitionsHash = 0;
    foreach (var (tileId, tileDefinition) in TileDefinitions)
      tileDefinitionsHash = unchecked(tileDefinitionsHash + HashCode.Combine(tileId, tileDefinition.GetHashCode()));
    hash.Add(tileDefinitionsHash);

    return hash.ToHashCode();
  }

  private static bool TileDefinitionsEqual(
    ImmutableDictionary<uint, TileDefinition> left,
    ImmutableDictionary<uint, TileDefinition> right)
  {
    if (left.Count != right.Count)
      return false;

    foreach (var (tileId, tileDefinition) in left)
      if (!right.TryGetValue(tileId, out var otherDefinition) || !tileDefinition.Equals(otherDefinition))
        return false;

    return true;
  }
}