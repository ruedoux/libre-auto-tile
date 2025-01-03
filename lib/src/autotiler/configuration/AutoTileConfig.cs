using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qwaitumin.AutoTile;

public class Vector2Converter : JsonConverter<Vector2>
{
  public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    => writer.WriteStringValue(Vector2ToString(value));

  private static Vector2 FromString(string text)
  {
    var parts = text.Trim('(', ')').Split(',');
    if (parts.Length != 2)
      throw new ArgumentException($"Unable to convert string to Vector2Int: {text}");
    return new Vector2(int.Parse(parts[0]), int.Parse(parts[1]));
  }

  private static string Vector2ToString(Vector2 v)
    => $"({v.X},{v.Y})";
}

[JsonSourceGenerationOptions(
  Converters = new Type[] { typeof(Vector2Converter) },
  DefaultIgnoreCondition = JsonIgnoreCondition.Never,
  IncludeFields = true)]
[JsonSerializable(typeof(AutoTileConfig))]
public partial class AutoTileConfigJsonContext : JsonSerializerContext { }

public class AutoTileConfig
{
  public int TileSize { get; private set; }
  public ImmutableDictionary<string, TileDefinition> TileDefinitions { get; private set; }
  public ImmutableDictionary<string, ImmutableDictionary<byte, Vector2>> BitmaskSets { get; private set; }

  [JsonConstructor]
  public AutoTileConfig(
    int tileSize,
    ImmutableDictionary<string, TileDefinition> tileDefinitions,
    ImmutableDictionary<string, ImmutableDictionary<byte, Vector2>> bitmaskSets
    )
  {
    TileSize = tileSize;
    TileDefinitions = tileDefinitions;
    BitmaskSets = bitmaskSets;

    IntegrityAssertion();
  }

  public static AutoTileConfig Construct(
    int tileSize,
    Dictionary<string, TileDefinition> tileDefinitions,
    Dictionary<string, Dictionary<byte, Vector2>> bitmaskSets)
  {
    return new(
      tileSize,
      tileDefinitions.ToImmutableDictionary(),
      bitmaskSets.ToImmutableDictionary(entry => entry.Key, entry => entry.Value.ToImmutableDictionary()));
  }

  public static AutoTileConfig? FromString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileConfigJsonContext.Default.AutoTileConfig)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public override string ToString()
    => JsonSerializer.Serialize(this, AutoTileConfigJsonContext.Default.AutoTileConfig);

  private void IntegrityAssertion()
  {
    foreach (var (tileName, tileDefinition) in TileDefinitions)
      if (!BitmaskSets.ContainsKey(tileDefinition.BitmaskName))
        throw new ArgumentException($"Missing required bitmask set: '{tileDefinition.BitmaskName}' in autoTileConfig, for tile definition: '{tileName}'");
  }
}