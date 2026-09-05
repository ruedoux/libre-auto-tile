using System.Collections.Immutable;
using System.Text.Json;

namespace Qwaitumin.LibreAutoTile.Configuration.Models;

public class TileMaskDefinition
{
  public readonly ImmutableDictionary<Vector3, ImmutableArray<TileMaskData>> AtlasPositionToTileMaskAndChance;

  public TileMaskDefinition(
    ImmutableDictionary<Vector3, ImmutableArray<TileMaskData>> atlasPositionToTileMaskAndChance)
  {
    AtlasPositionToTileMaskAndChance = atlasPositionToTileMaskAndChance;
    foreach (var (_, tileMasksAndChanceArray) in atlasPositionToTileMaskAndChance)
      foreach (var (Mask, Chance) in tileMasksAndChanceArray)
        if (Mask.Length != 8)
          throw new ArgumentException($"Tile mask length must be 8, but is: {Mask.Length}");
  }

  public static TileMaskDefinition Construct(
    Dictionary<Vector3, TileMaskData[]> atlasPositionToTileMaskAndChance)
  {
    var immutableAtlasPositionToTileMaskAndChance =
      atlasPositionToTileMaskAndChance.ToImmutableDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value
          .Select(x => new TileMaskData([.. x.Mask], x.Chance))
          .ToImmutableArray());

    return new TileMaskDefinition(immutableAtlasPositionToTileMaskAndChance);
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

    bool dictsEqual = AtlasPositionToTileMaskAndChance.Count == other.AtlasPositionToTileMaskAndChance.Count;
    if (!dictsEqual)
      return false;

    foreach (var (atlasPosition, tileMasksAndChance) in AtlasPositionToTileMaskAndChance)
    {
      if (!other.AtlasPositionToTileMaskAndChance.TryGetValue(atlasPosition, out var otherTileMasksAndChance))
        return false;

      if (otherTileMasksAndChance.Length != tileMasksAndChance.Length)
        return false;

      for (int i = 0; i < tileMasksAndChance.Length; i++)
      {
        if (tileMasksAndChance[i].Chance != otherTileMasksAndChance[i].Chance)
          return false;

        if (!tileMasksAndChance[i].Mask.SequenceEqual(otherTileMasksAndChance[i].Mask))
          return false;
      }
    }

    return true;
  }

  public override int GetHashCode()
  {
    int definitionsHash = 0;

    foreach (var (atlasPosition, tileMasksAndChance) in AtlasPositionToTileMaskAndChance)
    {
      var entryHash = new HashCode();
      entryHash.Add(atlasPosition);

      foreach (var (Mask, Chance) in tileMasksAndChance)
      {
        foreach (var item in Mask)
          entryHash.Add(item);
        entryHash.Add(Chance);
      }

      definitionsHash = unchecked(definitionsHash + entryHash.ToHashCode());
    }

    return definitionsHash;
  }
}