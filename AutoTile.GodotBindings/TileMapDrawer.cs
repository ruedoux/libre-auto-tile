using Godot;

namespace Qwaitumin.AutoTile.GodotBindings;

internal class TileMapDrawer(TileMapWrapper tileMapWrapper) : ITileMapDrawer
{
  private readonly TileMapWrapper tileMapWrapper = tileMapWrapper;

  public void Clear()
    => Callable.From(tileMapWrapper.TileMapLayer.Clear).CallDeferred();

  public void DrawTiles(
    int tileLayer, KeyValuePair<Configuration.Vector2, TileData>[] positionsToTileData)
  {
    Callable.From(() =>
    {
      foreach (var (position, tileData) in positionsToTileData)
      {
        if (!tileMapWrapper.TileIdToSourceId.TryGetValue(tileData.CentreTileId, out int sourceId))
          sourceId = -1;

        Vector2I? atlasCoords = GodotTypeMapper.Map(tileData.TileAtlas.Position);
        if (tileData.CentreTileId < 0 || tileData.TileAtlas.ImageFileName == string.Empty)
        {
          atlasCoords = null;
          sourceId = -1;
        }

        tileMapWrapper.TileMapLayer.SetCell(GodotTypeMapper.Map(position), sourceId, atlasCoords);
      }
    }).CallDeferred();
  }
}