using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling;

public static class AutoTileConfigurationExtractor
{
  public static Dictionary<uint, HashSet<int>> GetConnectionGroupToTileIds(
    AutoTileConfiguration autoTileConfiguration)
  => autoTileConfiguration.TileDefinitions
      .Where(td => td.Value.ConnectionGroup != null)
      .GroupBy(td => td.Value.ConnectionGroup!.Value)
      .ToDictionary(g => g.Key, g => new HashSet<int>(g.Select(td => (int)td.Key)));

  public static (TileMask TileMask, TileAtlas TileAtlas)[] GetItems(
    TileDefinition tileDefinition)
  {
    List<(TileMask TileMask, TileAtlas TileAtlas)> items = [];
    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (position, tileMaskAndChanceArray) in tileMaskDefinition.AtlasPositionToTileMaskAndChance)
      {
        foreach (var (mask, chance) in tileMaskAndChanceArray)
        {
          TileMask tileMask = TileMask.FromArray([.. mask]);
          TileAtlas tileAtlas = new(position.ToVector2(), imageFileName, chance);
          items.Add(new(tileMask, tileAtlas));
        }
      }
    }
    return [.. items];
  }

  public static Dictionary<int, TileSearcher> BuildTileIdToTileMaskSearcher(
    AutoTileConfiguration autoTileConfiguration, int cacheSize = 1024)
  {
    var connectionGroupToTileIds = GetConnectionGroupToTileIds(autoTileConfiguration);

    Dictionary<int, TileSearcher> tileIdToTileSearcher = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      var items = GetItems(tileDefinition);

      HashSet<int>? connectionGroupArray = null;
      if (tileDefinition.ConnectionGroup is not null)
        connectionGroupArray = connectionGroupToTileIds[(uint)tileDefinition.ConnectionGroup];

      TileMaskSearcher tileMaskSearcher = new(
        items.Select(x => x.TileMask),
        connectionGroupArray,
        autoTileConfiguration.WildcardId,
        cacheSize);
      TileAtlasResolver tileAtlasResolver = new(items);
      tileIdToTileSearcher.Add((int)tileId, new(tileMaskSearcher, tileAtlasResolver));
    }
    return tileIdToTileSearcher;
  }
}