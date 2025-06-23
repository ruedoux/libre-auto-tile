using Godot;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapDrawer(TileMapWrapper tileMapWrapper) : ITileMapDrawer
{
  private readonly TileMapWrapper tileMapWrapper = tileMapWrapper;

  public void Clear()
    => Callable.From(tileMapWrapper.TileMapLayer.Clear).CallDeferred();

  public void DrawTiles(
    int tileLayer, IEnumerable<(Configuration.Models.Vector2, Tiling.TileData)> positionsToTileData)
  {
    Callable.From(() =>
    {
      foreach (var (position, tileData) in positionsToTileData)
      {
        Vector2I? atlasCoords = null;
        var sourceId = -1;
        if (tileData.TileAtlas.ImageFileName is not null && tileData.TileAtlas.ImageFileName != "")
        {
          if (tileMapWrapper.ImageFileToSourceId.TryGetValue(tileData.TileAtlas.ImageFileName, out int overrideSourceId))
          {
            atlasCoords = GodotTypeMapper.Map(tileData.TileAtlas.Position);
            sourceId = overrideSourceId;
          }
        }

        tileMapWrapper.TileMapLayer.SetCell(GodotTypeMapper.Map(position), sourceId, atlasCoords);
      }
    }).CallDeferred();
  }
}