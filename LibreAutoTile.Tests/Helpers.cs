using System.Text.Json.Serialization;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile.Tests;

[JsonSourceGenerationOptions(
  Converters = [typeof(Vector2DictionaryConverter<int[]>)],
  DefaultIgnoreCondition = JsonIgnoreCondition.Never,
  IncludeFields = true)]
[JsonSerializable(typeof(Dictionary<Vector2, int[]>))]
public partial class AutoTileTestJsonContext : JsonSerializerContext { }