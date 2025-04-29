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
        // Ignores tile ids that have no source, validity up to discussion
        if (tileMapWrapper.TileIdToSourceId.TryGetValue(tileData.CentreTileId, out int sourceId))
          if (tileData.CentreTileId >= 0)
            tileMapWrapper.TileMapLayer.SetCell(
              GodotTypeMapper.Map(position),
              sourceId,
              GodotTypeMapper.Map(tileData.TileAtlas.Position));
          else
            tileMapWrapper.TileMapLayer.SetCell(GodotTypeMapper.Map(position));
      }
    }).CallDeferred();
  }
}