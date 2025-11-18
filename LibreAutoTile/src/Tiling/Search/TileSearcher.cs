namespace Qwaitumin.LibreAutoTile.Tiling.Search;


public class TileSearcher
{
  private readonly TileMaskSearcher tileMaskSearcher;
  private readonly TileAtlasResolver tileAtlasResolver;

  public TileSearcher(TileMaskSearcher tileMaskSearcher, TileAtlasResolver tileAtlasResolver)
  {
    this.tileMaskSearcher = tileMaskSearcher;
    this.tileAtlasResolver = tileAtlasResolver;
  }

  public (TileMask TileMask, TileAtlas TileAtlas) FindBestMatch(TileMask target)
  {
    var tileMask = tileMaskSearcher.FindBestMatch(target);
    var tileAtlas = tileAtlasResolver.GetTileAtlas(tileMask);
    return new(tileMask, tileAtlas);
  }
}