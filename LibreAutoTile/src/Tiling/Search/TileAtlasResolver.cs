using System.Collections.Frozen;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;


/// <summary>
/// Finds atlas for a provided mask. Thread safe.
/// </summary>
public class TileAtlasResolver
{
  private readonly FrozenDictionary<TileMask, (int Offset, int Count)> indexMap;
  private readonly TileAtlas[] atlasArray;

  public TileAtlasResolver(IEnumerable<(TileMask TileMask, TileAtlas TileAtlas)> items)
  {
    var groups = items
    .GroupBy(x => x.TileMask)
    .ToDictionary(
        g => g.Key,
        g =>
        {
          List<TileAtlas> atlases = [];
          foreach (var x in g.OrderBy(x => x.TileAtlas.Chance))
            atlases.Add(x.TileAtlas);
          // Force last .Chance to int.MaxValue to guarantee coverage
          atlases[^1] = atlases[^1] with { Chance = int.MaxValue };
          return atlases;
        }
    );

    var atlasList = new List<TileAtlas>();
    var mapBuilder = new Dictionary<TileMask, (int Offset, int Count)>();

    foreach (var kv in groups)
    {
      int offset = atlasList.Count;
      atlasList.AddRange(kv.Value);
      int count = kv.Value.Count;
      mapBuilder[kv.Key] = (offset, count);
    }

    atlasArray = [.. atlasList];
    indexMap = mapBuilder.ToFrozenDictionary();
  }

  public TileAtlas GetTileAtlas(TileMask tileMask)
  {
    if (!indexMap.TryGetValue(tileMask, out var index))
      return new TileAtlas();
    int randomNumber = Random.Shared.Next();
    int start = index.Offset;
    int count = index.Count;
    int end = start + count;
    int low = start, high = end - 1;
    while (low < high)
    {
      int mid = low + ((high - low) >> 1);
      if (atlasArray[mid].Chance > randomNumber)
        high = mid;
      else
        low = mid + 1;
    }
    return atlasArray[low];
  }
}