using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile;

public class AutoTilerComposer(
  AutoTileConfiguration autoTileConfiguration, bool throwIfNoImage = true)
{
  public AutoTiler GetAutoTiler(uint layerCount)
    => new(layerCount, GetTileMaskSearchers());

  public Dictionary<int, TileMaskSearcher> GetTileMaskSearchers()
  {
    Dictionary<int, TileMaskSearcher> autoTileIndexToTileDatas = [];

    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(Path.Join(imageFileName)) && throwIfNoImage)
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