using System.Collections.Frozen;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;


/// <summary>
/// Finds atlas for a provided mask. Thread safe.
/// </summary>
public class TileAtlasResolver
{
  private readonly FrozenDictionary<TileMask, (int Offset, int Count, ulong TotalChance)> indexMap;
  private readonly TileAtlas[] atlasArray;

  public TileAtlasResolver(IEnumerable<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    var grouped = new Dictionary<TileMask, List<TileAtlas>>();

    foreach (var (tileMask, tileAtlas) in items)
    {
      if (!grouped.TryGetValue(tileMask, out var list))
      {
        list = [];
        grouped[tileMask] = list;
      }
      list.Add(tileAtlas);
    }

    var atlasList = new List<TileAtlas>();
    var mapBuilder = new Dictionary<TileMask, (int Offset, int Count, ulong TotalChance)>(grouped.Count);

    foreach (var (mask, list) in grouped)
    {
      int offset = atlasList.Count;
      atlasList.AddRange(list);

      ulong totalChance = 0;
      foreach (var atlas in list)
        totalChance = unchecked(totalChance + atlas.Chance);

      mapBuilder[mask] = (offset, list.Count, totalChance);
    }

    atlasArray = [.. atlasList];
    indexMap = mapBuilder.ToFrozenDictionary();
  }

  public TileAtlas GetTileAtlas(TileMask tileMask)
  {
    if (!indexMap.TryGetValue(tileMask, out var index))
      return new TileAtlas();

    if (index.Count == 1 || index.TotalChance == 0)
      return atlasArray[index.Offset];

    ulong randomNumber = (ulong)Random.Shared.NextInt64((long)index.TotalChance);

    ulong currentChance = 0;
    for (int i = index.Offset; i < index.Offset + index.Count; i++)
    {
      currentChance = unchecked(currentChance + atlasArray[i].Chance);
      if (randomNumber < currentChance)
        return atlasArray[i];
    }

    return atlasArray[index.Offset + index.Count - 1];
  }
}
