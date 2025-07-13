using Godot;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapDrawer(TileMapWrapper[] tileMapWrappers) : ITileMapDrawer
{
  private readonly TileMapWrapper[] tileMapWrappers = tileMapWrappers;

  public void Clear()
  {
    Callable.From(() =>
    {
      foreach (var tileMapWrapper in tileMapWrappers)
        tileMapWrapper.TileMapLayer.Clear();
    }).CallDeferred();
  }

  public void DrawTiles(
    int tileLayer, IEnumerable<(Configuration.Models.Vector2, Tiling.TileData)> positionsToTileData)
  {
    if (tileLayer >= tileMapWrappers.Length)
      throw new ArgumentException($"No layer: {tileLayer}");

    Callable.From(() =>
    {
      foreach (var (position, tileData) in positionsToTileData)
      {
        Vector2I? atlasCoords = null;
        var sourceId = -1;
        if (tileData.TileAtlas.ImageFileName is not null && tileData.TileAtlas.ImageFileName != "")
        {
          if (tileMapWrappers[tileLayer].ImageFileToSourceId.TryGetValue(tileData.TileAtlas.ImageFileName, out int overrideSourceId))
          {
            atlasCoords = GodotTypeMapper.Map(tileData.TileAtlas.Position);
            sourceId = overrideSourceId;
          }
        }

        tileMapWrappers[tileLayer].TileMapLayer.SetCell(GodotTypeMapper.Map(position), sourceId, atlasCoords);
      }
    }).CallDeferred();
  }

  public int GetSourceId(string imageFileName)
  {
    int sourceId = -1;
    if (tileMapWrappers[0].ImageFileToSourceId.TryGetValue(imageFileName, out int foundSourceId))
      return foundSourceId;
    return sourceId;
  }
}