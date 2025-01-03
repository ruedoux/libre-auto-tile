using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Qwaitumin.LibreAutoTile.Configuration;

public class Vector2Converter : JsonConverter<Vector2>
{
  public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return Vector2.FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    => writer.WriteStringValue(value.ToString());
}

public class Vector2DictionaryConverter<TValue> : JsonConverter<Dictionary<Vector2, TValue>>
{
  public override Dictionary<Vector2, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadDictionary<Vector2, TValue>(
      ref reader, options, Vector2.FromString);

  public override void Write(Utf8JsonWriter writer, Dictionary<Vector2, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}

public class Vector2ImmutableDictionaryConverter<TValue> : JsonConverter<ImmutableDictionary<Vector2, TValue>>
{
  public override ImmutableDictionary<Vector2, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadImmutableDictionary<Vector2, TValue>(
      ref reader, options, Vector2.FromString);

  public override void Write(Utf8JsonWriter writer, ImmutableDictionary<Vector2, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}

public class Vector3Converter : JsonConverter<Vector3>
{
  public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
      throw new JsonException("Expected string token");

    var stringValue = reader!.GetString()
      ?? throw new JsonException("Token is null");

    return Vector3.FromString(stringValue);
  }

  public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
    => writer.WriteStringValue(value.ToString());
}

public class Vector3DictionaryConverter<TValue> : JsonConverter<Dictionary<Vector3, TValue>>
{
  public override Dictionary<Vector3, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadDictionary<Vector3, TValue>(
      ref reader, options, Vector3.FromString);

  public override void Write(Utf8JsonWriter writer, Dictionary<Vector3, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}

public class Vector3ImmutableDictionaryConverter<TValue> : JsonConverter<ImmutableDictionary<Vector3, TValue>>
{
  public override ImmutableDictionary<Vector3, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  => JsonSerialization.ReadImmutableDictionary<Vector3, TValue>(
      ref reader, options, Vector3.FromString);

  public override void Write(Utf8JsonWriter writer, ImmutableDictionary<Vector3, TValue> value, JsonSerializerOptions options)
    => JsonSerialization.Write(writer, value, options, (k) => k.ToString());
}

public class TileColorConverter : JsonConverter<TileColor>
{
  public override TileColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string colorString = reader.GetString()
      ?? throw new JsonException("Value deserialization results in null");
    string[] values = colorString.Trim('(', ')').Split(',');
    byte r = byte.Parse(values[0].Trim());
    byte g = byte.Parse(values[1].Trim());
    byte b = byte.Parse(values[2].Trim());
    byte a = byte.Parse(values[3].Trim());
    return new(r: r, g: g, b: b, a: a);
  }

  public override void Write(Utf8JsonWriter writer, TileColor value, JsonSerializerOptions options)
    => writer.WriteStringValue($"({value.R},{value.G},{value.B},{value.A})");
}

[JsonSourceGenerationOptions(
  Converters = [
    typeof(Vector2Converter),
    typeof(Vector3ImmutableDictionaryConverter<ImmutableArray<ImmutableArray<int>>>),
    typeof(TileColorConverter),
    typeof(Vector3Converter),
  ],
  DefaultIgnoreCondition = JsonIgnoreCondition.Never,
  IncludeFields = true)]
[JsonSerializable(typeof(AutoTileConfiguration))]
[JsonSerializable(typeof(TileDefinition))]
[JsonSerializable(typeof(TileMaskDefinition))]
public partial class AutoTileJsonContext : JsonSerializerContext { }

public static class JsonSerialization
{
  public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
    ref Utf8JsonReader reader,
    JsonSerializerOptions options,
    Func<string, TKey> keyDeserialization) where TKey : notnull
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException("Expected start object token");

    var dictionary = new Dictionary<TKey, TValue>();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return dictionary;

      string keyString = reader.GetString() ?? throw new JsonException();
      reader.Read();

      TValue value = JsonSerializer.Deserialize(ref reader, typeInfo)
        ?? throw new JsonException("Value deserialization results in null");
      dictionary.Add(keyDeserialization(keyString), value);
    }

    throw new JsonException("Object did not end with end token");
  }

  public static ImmutableDictionary<TKey, TValue> ReadImmutableDictionary<TKey, TValue>(
    ref Utf8JsonReader reader,
    JsonSerializerOptions options,
    Func<string, TKey> keyDeserialization) where TKey : notnull
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException("Expected start object token");

    var dictionary = new Dictionary<TKey, TValue>();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return dictionary.ToImmutableDictionary();

      string keyString = reader.GetString() ?? throw new JsonException();
      reader.Read();
      TValue value = JsonSerializer.Deserialize(ref reader, typeInfo)
        ?? throw new JsonException("Value deserialization results in null");
      dictionary.Add(keyDeserialization(keyString), value);
    }

    throw new JsonException("Object did not end with end token");
  }

  public static void Write<TKey, TValue>(
    Utf8JsonWriter writer,
    Dictionary<TKey, TValue> value,
    JsonSerializerOptions options,
    Func<TKey, string> KeySerialization) where TKey : notnull
  {
    writer.WriteStartObject();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    foreach (var kvp in value)
    {
      writer.WritePropertyName(KeySerialization(kvp.Key));
      JsonSerializer.Serialize(writer, kvp.Value, typeInfo);
    }

    writer.WriteEndObject();
  }

  public static void Write<TKey, TValue>(
    Utf8JsonWriter writer,
    ImmutableDictionary<TKey, TValue> value,
    JsonSerializerOptions options,
    Func<TKey, string> KeySerialization) where TKey : notnull
  {
    writer.WriteStartObject();
    var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

    foreach (var kvp in value)
    {
      writer.WritePropertyName(KeySerialization(kvp.Key));
      JsonSerializer.Serialize(writer, kvp.Value, typeInfo);
    }

    writer.WriteEndObject();
  }
}