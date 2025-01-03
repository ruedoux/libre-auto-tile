using System.Collections.Immutable;
using System.Text.Json;

namespace Qwaitumin.AutoTile.Configuration;

public sealed class TileMaskDefinition
{
  public readonly ImmutableDictionary<Vector2, ImmutableArray<ImmutableArray<int>>> AtlasPositionToTileMasks;

  public TileMaskDefinition(
    ImmutableDictionary<Vector2, ImmutableArray<ImmutableArray<int>>> atlasPositionToTileMasks)
  {
    AtlasPositionToTileMasks = atlasPositionToTileMasks;
    foreach (var (_, tileMasks) in atlasPositionToTileMasks)
      foreach (var tileMask in tileMasks)
        if (tileMask.Length != 8)
          throw new ArgumentException($"Tile mask length must be 8, but is: {tileMask.Length}");
  }

  public static TileMaskDefinition Construct(
    Dictionary<Vector2, int[][]> atlasPositionToTileMasks)
  {
    var immutableAtlasPositionToTileMasks = atlasPositionToTileMasks.ToImmutableDictionary(
      kvp => kvp.Key,
      kvp => kvp.Value
        .Select(innerArray => innerArray.ToImmutableArray())
        .ToImmutableArray());
    return new(atlasPositionToTileMasks: immutableAtlasPositionToTileMasks);
  }

  public static TileMaskDefinition? FromJsonString(string jsonString)
  {
    var deserialized = JsonSerializer.Deserialize(jsonString, AutoTileJsonContext.Default.TileMaskDefinition)
      ?? throw new ArgumentException($"Deserialization results in null for string: {jsonString}");
    return deserialized;
  }

  public string ToJsonString()
    => JsonSerializer.Serialize(this, AutoTileJsonContext.Default.TileMaskDefinition);


  public override bool Equals(object? obj)
  {
    if (obj is not TileMaskDefinition other)
      return false;

    bool dictsEqual = AtlasPositionToTileMasks.Count == other.AtlasPositionToTileMasks.Count;
    if (!dictsEqual)
      return false;

    foreach (var (atlasPosition, tileMasks) in AtlasPositionToTileMasks)
    {
      if (!other.AtlasPositionToTileMasks.TryGetValue(atlasPosition, out var otherTileMasks))
        return false;
      if (otherTileMasks.Length != tileMasks.Length)
        return false;
      for (int tileMasksIndex = 0; tileMasksIndex < tileMasks.Length; tileMasksIndex++)
        if (!otherTileMasks[tileMasksIndex].SequenceEqual(tileMasks[tileMasksIndex]))
          return false;
    }

    return true;
  }

  public override int GetHashCode()
  {
    var hash = new HashCode();
    foreach (var kvp in AtlasPositionToTileMasks)
    {
      hash.Add(kvp.Key);
      foreach (var array in kvp.Value)
        foreach (var item in array)
          hash.Add(item);
    }

    return hash.ToHashCode();
  }
}