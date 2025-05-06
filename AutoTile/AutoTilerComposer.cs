using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile;

public class AutoTilerComposer(AutoTileConfiguration autoTileConfiguration)
{
  public AutoTiler GetAutoTiler(uint layerCount)
    => new(layerCount, GetTileMaskSearchers());

  private Dictionary<int, TileMaskSearcher> GetTileMaskSearchers()
  {
    Dictionary<int, TileMaskSearcher> autoTileIndexToTileDatas = [];

    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(Path.Join(imageFileName)))
          throw new DirectoryNotFoundException($"Image does not exist: {imageFileName}");

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      List<(TileMask, TileAtlas)> tileMaskSearcherItems = [];
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
        foreach (var (position, tileMaskArrays) in tileMaskDefinition.AtlasPositionToTileMasks)
          foreach (var tileMaskArray in tileMaskArrays)
            tileMaskSearcherItems.Add(
              new(TileMask.FromArray([.. tileMaskArray]), new(position.ToVector2(), imageFileName)));
      autoTileIndexToTileDatas.Add((int)tileId, new(tileMaskSearcherItems));
    }

    return autoTileIndexToTileDatas;
  }
}