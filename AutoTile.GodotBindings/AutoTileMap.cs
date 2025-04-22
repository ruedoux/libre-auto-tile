using Godot;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper tileMapWrapper;
  private readonly AutoTileDrawer autoTileDrawer;

  public AutoTileMap(
    uint layerCount,
    string imageDirectoryPath,
    AutoTileConfiguration autoTileConfiguration)
  {
    AutoTilerComposer autoTilerComposer = new(imageDirectoryPath, autoTileConfiguration);
    tileMapWrapper = new(imageDirectoryPath, autoTileConfiguration);
    autoTileDrawer = new(
      new TileMapDrawer(tileMapWrapper),
      autoTilerComposer.GetAutoTiler(layerCount));

    AddChild(tileMapWrapper.TileMapLayer);
  }

  public void Clear()
    => autoTileDrawer.Clear();

  public void Wait()
    => autoTileDrawer.Wait();

  public void DrawTilesAsync(int layer, KeyValuePair<Vector2I, int>[] positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(kvp => new KeyValuePair<Qwaitumin.AutoTile.Configuration.Vector2, int>(
          GodotTypeMapper.Map(kvp.Key),
          kvp.Value))
      .ToArray();
    autoTileDrawer.DrawTilesAsync(layer, positionToTileIdsConverted);
  }

  public void DrawTiles(int layer, KeyValuePair<Vector2I, int>[] positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(kvp => new KeyValuePair<Qwaitumin.AutoTile.Configuration.Vector2, int>(
          GodotTypeMapper.Map(kvp.Key),
          kvp.Value))
      .ToArray();
    autoTileDrawer.DrawTiles(layer, positionToTileIdsConverted);
  }

  public void UpdateTiles(int tileLayer, Vector2I[] positions)
  {
    var positionsConverted = positions.Select(GodotTypeMapper.Map).ToArray();
    autoTileDrawer.UpdateTiles(tileLayer, positionsConverted);
  }

  public TileMapLayer GetTileMapLayer()
    => tileMapWrapper.TileMapLayer;

  public new void QueueFree()
  {
    tileMapWrapper.TileMapLayer.QueueFree();
    base.QueueFree();
  }
}