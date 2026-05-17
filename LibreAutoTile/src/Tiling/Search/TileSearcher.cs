using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling.Search;

/// <summary>
/// Finds best fitting mask and atlas. Thread safe.
/// </summary>
public class TileSearcher(TileMaskSearcher tileMaskSearcher, TileAtlasResolver tileAtlasResolver)
{
  private readonly TileMaskSearcher tileMaskSearcher = tileMaskSearcher;
  private readonly TileAtlasResolver tileAtlasResolver = tileAtlasResolver;

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    var tileMask = tileMaskSearcher.FindBestMatch(target);
    var tileAtlas = tileAtlasResolver.GetTileAtlas(tileMask);
    return new(tileMask, tileAtlas);
  }
}